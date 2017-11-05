public sealed partial class MainPage : Page
{
	// Bluetooth Beacons
	private readonly BluetoothLEAdvertisementWatcher watcher;
	private readonly BeaconManager beaconManager;
	
	private const int rssiValidity = 10;

	public MainPage()
	{
		// Construct the Universal Bluetooth Beacon manager
		beaconManager = new BeaconManager();

		// Create & start the Bluetooth LE watcher
		watcher = new BluetoothLEAdvertisementWatcher { ScanningMode = BluetoothLEScanningMode.Passive};
		watcher.Received += WatcherOnReceived;   
		watcher.Start();
	}

	protected override async void OnNavigatedTo(NavigationEventArgs e)
	{
	   while(true)
		{
			foreach (var b in CommunicationHelper.Beacons)
			{
				if((DateTime.Now - b.UpdatedAt).Seconds > rssiValidity)
					//Remove value, and wait for the new one
					b.Distance = 0;
			}
			
			//Builds the final EventHub payload containing other metadata
			await CommunicationHelper.ReportBLESignalIntensity();
			
			//Run in two second window
			await Task.Delay(2000);
		}
	}


	private async void WatcherOnReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
	{
		//We need to create HEX representation of Mac Address split with ':'
		string addr = Regex.Replace(eventArgs.BluetoothAddress.ToString("X"), ".{2}", "$0:");
		addr = addr.Remove(addr.Length - 1, 1);
		var beacon = CommunicationHelper.Beacons.Where(b => b.Device == addr).FirstOrDefault();
		if (beacon != null){
			beacon.Distance = eventArgs.RawSignalStrengthInDBm; 
			beacon.UpdatedAt = DateTime.Now;
		}			
	}	
}