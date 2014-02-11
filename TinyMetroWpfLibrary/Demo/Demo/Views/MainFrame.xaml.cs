using BoonieBear.TinyMetro.WPF.Controller;

namespace Demo.Views
{

    public partial class MainFrame
    {
        public MainFrame()
        {
            InitializeComponent();
            Kernel.Instance.Controller.SetRootFrame(ContentFrame);
        }
    }
}
