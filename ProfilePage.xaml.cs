namespace MyCar;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
	}

    private void Home_Tapped(object sender, TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new MainScreenPage();
        }  
    }

    private void Garage_Tapped(object sender, TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new GaragePage();
        }
    }

    private void Service_Tapped(object sender, TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new ServicePage();
        }
    }

    private void News_Tapped(object sender, TappedEventArgs e)
    {
        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new NewsPage();
        }
    }

    private void Profile_Tapped(object sender, TappedEventArgs e)
    {
        
    }
}