namespace MyCar
{
    public partial class MainPage : ContentPage
    {
        private IDispatcherTimer? _timer;

        public MainPage()
        {
            InitializeComponent();
            LoadVideo();
        }

        private void LoadVideo()
        {
            string videoPath = "file:///android_asset/zastavka.mp4";

            string html = $@"<html>
<body style='margin:0; padding:0; background:black;'>
    <video id='vid' width='100%' height='100%' 
           preload='auto' playsinline
           poster='file:///android_asset/poster.png'
           style='object-fit:cover;'>
        <source src='{videoPath}' type='video/mp4'>
    </video>
</body>
</html>";

            VideoWebView.Source = new HtmlWebViewSource { Html = html };
        }

        private async void OnStartClicked(object sender, EventArgs e)
        {
            StartButton.IsVisible = false;

            await VideoWebView.EvaluateJavaScriptAsync("document.getElementById('vid').play();");

            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(5); 
            _timer.Tick += (s, args) =>
            {
                _timer.Stop();
                ShowAuthButtons();
            };
            _timer.Start();
        }

        private async void ShowAuthButtons()
        {
            AuthButtons.IsVisible = true;
            await AuthButtons.FadeToAsync(1, 500);
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
}
