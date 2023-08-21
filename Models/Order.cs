using Stateless;
using System.ComponentModel.DataAnnotations;

namespace FakeXiecheng.API.Models
{
    //订单状态
    public enum OrderStateEnum
    {
        Pending,//订单已生成
        Processing,//支付处理中
        Completed,//支付成功
        Declined,//支付失败
        Cancelled,//订单取消
        Refund,//已退款
    }

    //订单状态触发动作
    public enum OrderStateTriggerEnum
    {
        PalaceOrder,//支付
        Approve,//支付成功
        Reject,//支付失败
        Cancel,//取消
        Return,//退款
    }


    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> OrderItems { get; set; }
        public OrderStateEnum State { get; set; }//订单目前状态
        public DateTime CreateDateUTC { get; set; }
        public string TransactionMetadata { get; set; }//第三方支付数据
        //使用Stateless框架实现订单状态机
        private StateMachine<OrderStateEnum, OrderStateTriggerEnum> _machine;

        public void PaymentProcessing()
        {
            _machine.Fire(OrderStateTriggerEnum.PalaceOrder);
        }
        public void PaymentApprove()
        {
            _machine.Fire(OrderStateTriggerEnum.Approve);
        }
        public void PaymentReject()
        {
            _machine?.Fire(OrderStateTriggerEnum.Reject);
        }

        public Order()
        {
            StateMachineInit();
        }

        //状态机初始化函数
        private void StateMachineInit()
        {
            _machine = new StateMachine<OrderStateEnum, OrderStateTriggerEnum>(() => State, s => State = s);
            
            //配置触发动作和状态转换
            _machine.Configure(OrderStateEnum.Pending)
                .Permit(OrderStateTriggerEnum.PalaceOrder, OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Cancel, OrderStateEnum.Cancelled);
            _machine.Configure(OrderStateEnum.Processing)
                .Permit(OrderStateTriggerEnum.Approve, OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Reject, OrderStateEnum.Declined);
            _machine.Configure(OrderStateEnum.Completed)
                .Permit(OrderStateTriggerEnum.Return, OrderStateEnum.Refund);
        }


    }
}
