using Demo.NavigationEvents;
using BoonieBear.TinyMetro.WPF.Controller;
using BoonieBear.TinyMetro.WPF.EventAggregation;

namespace Demo.Core
{
    /// <summary>
    /// 消息处理函数
    /// </summary>
    internal class TestController : BaseController,
        IHandleMessage<GoPage1NavigationRequest>,
        IHandleMessage<GoPage2NavigationRequest>,
        IHandleMessage<GoMainPageNavigationRequest>
    {
        public void Handle(GoPage1NavigationRequest message)
        {
            NavigateToPage("Views/Page1View.xaml");
        }

        public void Handle(GoPage2NavigationRequest message)
        {
            NavigateToPage("Views/Page2View.xaml");
        }

        public void Handle(GoMainPageNavigationRequest message)
        {
            NavigateToPage("Views/MainPageView.xaml");
        }
    }
}