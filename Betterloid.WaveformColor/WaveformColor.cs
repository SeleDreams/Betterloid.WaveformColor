using Betterloid;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Media;
#if VOCALOID6
using Yamaha.VOCALOID;
using Yamaha.VOCALOID.MusicalEditor;
using Yamaha.VOCALOID.VSM;
using VOCALOID = Yamaha.VOCALOID;
#elif VOCALOID5
using Yamaha.VOCALOID.VOCALOID5;
using Yamaha.VOCALOID.VOCALOID5.MusicalEditor;
using Yamaha.VOCALOID.VSM;
using VOCALOID = Yamaha.VOCALOID.VOCALOID5;
#endif

namespace WaveformColor
{
    public class WaveformColor : IPlugin
    {
        public static Color DarkenColor(Color color, double factor)
        {
            // Ensure the factor is between 0 and 1
            factor = Math.Max(0, Math.Min(1, factor));

            // Calculate the new RGB values
            byte r = (byte)(color.R * factor);
            byte g = (byte)(color.G * factor);
            byte b = (byte)(color.B * factor);

            // Return the darkened color
            return Color.FromRgb(r, g, b);
        }

        static SolidColorBrush newBrush = new SolidColorBrush(Colors.Yellow);
        public void Startup()
        {
            MainWindow window = Application.Current.MainWindow as MainWindow;
            try
            {
                var xMusicalEditorDiv = window.FindName("xMusicalEditorDiv") as MusicalEditorDivision;
                var musicalEditor = xMusicalEditorDiv.DataContext as MusicalEditorViewModel;
                var zoomScrollViewer = xMusicalEditorDiv.FindName("xPianorollViewer") as ZoomScrollViewer;
                var view = zoomScrollViewer.Content as PianorollView;
                var brushField = view.GetType().GetField("brushRenderedWave", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var penfield = view.GetType().GetField("penRenderedWave", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                SolidColorBrush newBrush = new SolidColorBrush();
                Pen newPen = new Pen(newBrush, 1.0);
                
                musicalEditor.UpdateViewEvent += (object sender2, VOCALOID.MusicalEditor.UpdateViewTypeFlag typeFlags, UpdateObserverNotifyEventArgs observer, object addition) =>
                {
                    if (musicalEditor.ActiveTrack != null )
                    {
                        newBrush.Color = TrackColorTable.GetTrackColor(musicalEditor.ActiveTrack.Color);
                        newBrush.Color = DarkenColor(newBrush.Color, 0.4);
                        penfield.SetValue(view, newPen);
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBoxDeliverer.GeneralError("An error occured while setting the color of the waveforms ! " + ex.GetType().ToString() + " : " + ex.Message);
            }
        }
    }
}
