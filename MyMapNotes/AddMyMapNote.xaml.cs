using MyMapNotes.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace MyMapNotes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddMyMapNote : Page
    {
        private bool isViewing = false;
        private MapNote mapNote;
        public AddMyMapNote()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            Geopoint myPoint;

            if (e.Parameter == null)
            {
                isViewing = false;
                //ADD
                var locator = new Geolocator();
                locator.DesiredAccuracyInMeters = 50;

                var position = await locator.GetGeopositionAsync();
                myPoint = position.Coordinate.Point;
            }
            else
            {
                isViewing = true;
                //View or Delete
                mapNote = (MapNote)e.Parameter;
                titleTextBox.Text = mapNote.Title;
                noteTextBox.Text = mapNote.Note;
                addButton.Content = "Delete";

                var myPosition = new Windows.Devices.Geolocation.BasicGeoposition();
                myPosition.Latitude = mapNote.Latitude;
                myPosition.Longitude = mapNote.Longitude;

                myPoint = new Geopoint(myPosition);

            }
   
            await MyMap.TrySetViewAsync(myPoint, 16D);

        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (isViewing)
            {
                //Delete
                //pop up dialog box
                //var messageDialog = new Windows.UI.Popups.MessageDialog("Are you sure");
                // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                // messageDialog.Commands.Add(new UICommand("Delete", new UICommandInvokedHandler(this.CommandInvokedHandler)));

                //messageDialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(this.CommandInvokedHandler)));
                // Set the command that will be invoked by default

                MessageDialog msg = new MessageDialog("Are you Sure");

                //Commands
                msg.Commands.Add(new UICommand("Delete", new UICommandInvokedHandler(CommandHandlers)));
                msg.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(CommandHandlers)));

                msg.DefaultCommandIndex = 0;
                // Set the command to be invoked when escape is pressed
                msg.CancelCommandIndex = 1;
                // Show the message dialog
                await msg.ShowAsync();
                
            }
            else
            {

                //ADD
                MapNote newMapNote = new MapNote();
                newMapNote.Title = titleTextBox.Text;
                newMapNote.Note = noteTextBox.Text;
                newMapNote.Created = DateTime.Now;
                newMapNote.Latitude = MyMap.Center.Position.Latitude;
                newMapNote.Longitude = MyMap.Center.Position.Longitude;
                App.DataModel.AddMapNote(newMapNote);
                Frame.Navigate(typeof(MainPage));

            }
           
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        public void CommandHandlers(IUICommand command)
        {
            if(command.Label == "Delete")
            {
                App.DataModel.DeleteMapNote(mapNote);
                Frame.Navigate(typeof(MainPage));
            }
        }
    }
}
