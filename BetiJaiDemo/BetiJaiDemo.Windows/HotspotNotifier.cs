using System.Windows.Forms;

namespace BetiJaiDemo.Windows
{
    public class HotspotNotifier : IHotspotNotifier
    {
        public void Notify(string name)
        {
            MessageBox.Show(
                @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce suscipit, quam non faucibus tincidunt,
nulla lorem pulvinar odio, vel molestie ante urna non purus. Donec in maximus metus, ut convallis urna.
Donec tincidunt bibendum purus, et tempus tellus facilisis nec. Donec in magna eget nisi efficitur
blandit. Pellentesque auctor sem sed tortor gravida pellentesque. Cras tortor tortor, ornare non nunc sit
amet, tincidunt vulputate ex. Mauris vitae fringilla sapien. Curabitur volutpat elit sed tortor
vestibulum, in lacinia est faucibus. Nulla vehicula congue leo sit amet fermentum.", name);
        }
    }
}