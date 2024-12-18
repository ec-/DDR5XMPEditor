﻿using DDR5XMPEditor.DDR5SPD;
using DDR5XMPEditor.Events;
using Stylet;
using System.IO;
using System.Windows;

namespace DDR5XMPEditor.Pages
{
    public class DDR5MainViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<SelectedSPDFileEvent>,
        IHandle<SaveSPDFileEvent>
    {
        public DDR5_SPD DDR5_SPD { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public bool IsSPDValid => DDR5_SPD != null;

        private readonly DDR5SPDEditorViewModel ddr5spdVM;
        private readonly XMP3EditorViewModel xmpVm1, xmpVm2, xmpVm3, xmpVm4, xmpVm5;
        private readonly EXPOEditorViewModel expoVm1, expoVm2;
        private readonly DDR5MiscViewModel miscVm = new DDR5MiscViewModel { DisplayName = "Misc" };

        public DDR5MainViewModel(IEventAggregator aggregator)
        {
            aggregator.Subscribe(this);
            Items.Add(ddr5spdVM = new DDR5SPDEditorViewModel { DisplayName = "SPD" });
            Items.Add(xmpVm1 = new XMP3EditorViewModel(1) { DisplayName = "XMP 1" });
            Items.Add(xmpVm2 = new XMP3EditorViewModel(2) { DisplayName = "XMP 2" });
            Items.Add(xmpVm3 = new XMP3EditorViewModel(3) { DisplayName = "XMP 3" });
            Items.Add(xmpVm4 = new XMP3EditorViewModel(4) { DisplayName = "XMP User 1" });
            Items.Add(xmpVm5 = new XMP3EditorViewModel(5) { DisplayName = "XMP User 2" });
            Items.Add(expoVm1 = new EXPOEditorViewModel(1) { DisplayName = "EXPO 1" });
            Items.Add(expoVm2 = new EXPOEditorViewModel(1) { DisplayName = "EXPO 2" });
            Items.Add(miscVm);
            ActiveItem = Items[0];
        }

        public void Handle(SelectedSPDFileEvent e)
        {
            long length = new System.IO.FileInfo(e.FilePath).Length;

            // Check file size before loading contents
            if (length != DDR5_SPD.TotalSize)
            {
                System.Windows.MessageBox.Show("Invalid SPD file, file size must be 1024 bytes", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            byte[] spd_bytes = File.ReadAllBytes(e.FilePath);

            var spd = DDR5_SPD.Parse(spd_bytes);
            if (spd == null)
            {
                System.Windows.MessageBox.Show("Error parsing DDR5 SPD file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                DDR5_SPD = spd;

                ddr5spdVM.JedecProfile = DDR5_SPD;
                BindNotifyPropertyChanged(ddr5spdVM);

                DDR5_SPD.Bind(x => x.XMP1Enabled, (s, args) => xmpVm1.IsEnabled = args.NewValue);
                xmpVm1.XMPProfile = DDR5_SPD.XMP1;
                xmpVm1.SPD = spd;
                xmpVm1.IsEnabled = DDR5_SPD.XMP1Enabled;
                BindNotifyPropertyChanged(xmpVm1);

                DDR5_SPD.Bind(x => x.XMP2Enabled, (s, args) => xmpVm2.IsEnabled = args.NewValue);
                xmpVm2.XMPProfile = DDR5_SPD.XMP2;
                xmpVm2.SPD = spd;
                xmpVm2.IsEnabled = DDR5_SPD.XMP2Enabled;
                BindNotifyPropertyChanged(xmpVm2);

                DDR5_SPD.Bind(x => x.XMP3Enabled, (s, args) => xmpVm3.IsEnabled = args.NewValue);
                xmpVm3.XMPProfile = DDR5_SPD.XMP3;
                xmpVm3.SPD = spd;
                xmpVm3.IsEnabled = DDR5_SPD.XMP3Enabled;
                BindNotifyPropertyChanged(xmpVm3);

                DDR5_SPD.Bind(x => x.XMPUser1Enabled, (s, args) => xmpVm4.IsEnabled = args.NewValue);
                xmpVm4.XMPProfile = DDR5_SPD.XMPUser1;
                xmpVm4.SPD = spd;
                xmpVm4.IsEnabled = DDR5_SPD.XMPUser1Enabled;
                BindNotifyPropertyChanged(xmpVm4);

                DDR5_SPD.Bind(x => x.XMPUser2Enabled, (s, args) => xmpVm5.IsEnabled = args.NewValue);
                xmpVm5.XMPProfile = DDR5_SPD.XMPUser2;
                xmpVm5.SPD = spd;
                xmpVm5.IsEnabled = DDR5_SPD.XMPUser2Enabled;
                BindNotifyPropertyChanged(xmpVm5);

                DDR5_SPD.Bind(x => x.EXPO1Enabled, (s, args) => expoVm1.IsEnabled = args.NewValue);
                expoVm1.EXPOProfile = DDR5_SPD.EXPO1;
                expoVm1.SPD = spd;
                expoVm1.IsEnabled = DDR5_SPD.expoFound;
                BindNotifyPropertyChanged(expoVm1);

                DDR5_SPD.Bind(x => x.EXPO2Enabled, (s, args) => expoVm2.IsEnabled = args.NewValue);
                expoVm2.EXPOProfile = DDR5_SPD.EXPO2;
                expoVm2.SPD = spd;
                expoVm2.IsEnabled = DDR5_SPD.expoFound;
                BindNotifyPropertyChanged(expoVm2);

                FilePath = e.FilePath;
                FileName = System.IO.Path.GetFileName(FilePath);

                // Enable Misc tab
                miscVm.SPD = spd;
                miscVm.IsEnabled = true;
            }

        }

        public void Handle(SaveSPDFileEvent e)
        {
            DDR5_SPD.UpdateCrc();
            var bytes = DDR5_SPD.GetBytes();
            File.WriteAllBytes(e.FilePath, bytes);
            System.Windows.MessageBox.Show(
                $"Successfully saved DDR5 SPD to {e.FilePath}",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BindNotifyPropertyChanged(DDR5SPDEditorViewModel vm)
        {
            // Bind CAS latencies
            vm.JedecProfile.Bind(x => x.CL20, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL22, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL24, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL26, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL28, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL30, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL32, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL34, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL36, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL38, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL40, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL42, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL44, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL46, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL48, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL50, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL52, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL54, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL56, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL58, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL60, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL62, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL64, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL66, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL68, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL70, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL72, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL74, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL76, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL78, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL80, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL82, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL84, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL86, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL88, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL90, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL92, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL94, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL96, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.CL98, (s, e) => vm.Refresh());

            // Bind timings
            vm.JedecProfile.Bind(x => x.MinCycleTime, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tAA, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tAATicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tRCD, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRCDTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tRP, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRPTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tRAS, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRASTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tRC, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRCTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tWR, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tWRTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tRFC1_slr, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRFC1_slrTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tRFC2_slr, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRFC2_slrTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tRFCsb_slr, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRFCsb_slrTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tRFC1_dlr, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRFC1_dlrTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tRFC2_dlr, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRFC2_dlrTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tRFCsb_dlr, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRFCsb_dlrTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tRRD_L, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRRD_L_lowerLimit, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRRD_LTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tCCD_L, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_L_lowerLimit, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_LTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tCCD_L_WR, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_L_WR_lowerLimit, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_L_WRTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tCCD_L_WR2, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_L_WR2_lowerLimit, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_L_WR2Ticks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tFAW, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tFAW_lowerLimit, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tFAWTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tCCD_L_WTR, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_L_WTR_lowerLimit, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_L_WTRTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tCCD_S_WTR, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_S_WTR_lowerLimit, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_S_WTRTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tRTP, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRTP_lowerLimit, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tRTPTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tCCD_M, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_M_lowerLimit, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_MTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tCCD_M_WR, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_M_WR_lowerLimit, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_M_WRTicks, (s, e) => vm.Refresh());

            vm.JedecProfile.Bind(x => x.tCCD_M_WTR, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_M_WTR_lowerLimit, (s, e) => vm.Refresh());
            vm.JedecProfile.Bind(x => x.tCCD_M_WTRTicks, (s, e) => vm.Refresh());
        }
        private void BindNotifyPropertyChanged(XMP3EditorViewModel vm)
        {
            // Bind CAS latencies
            vm.XMPProfile.Bind(x => x.CL20, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL22, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL24, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL26, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL28, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL30, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL32, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL34, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL36, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL38, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL40, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL42, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL44, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL46, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL48, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL50, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL52, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL54, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL56, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL58, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL60, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL62, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL64, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL66, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL68, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL70, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL72, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL74, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL76, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL78, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL80, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL82, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL84, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL86, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL88, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL90, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL92, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL94, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL96, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CL98, (s, e) => vm.Refresh());

            // Bind timings
            vm.SPD.Bind(x => x.XMPProfile1Name, (s, e) => vm.Refresh());
            vm.SPD.Bind(x => x.XMPProfile2Name, (s, e) => vm.Refresh());
            vm.SPD.Bind(x => x.XMPProfile3Name, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.CommandRate, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.IntelDynamicMemoryBoost, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.RealTimeMemoryFrequencyOC, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.VDD, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.VDDQ, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.VPP, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.VMEMCTRL, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.MinCycleTime, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tAA, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tRCD, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tRP, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tRAS, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tRC, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tWR, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tRFC1, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tRFC2, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tRFC, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tRRD_L, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tRRD_L_lowerLimit, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tCCD_L, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tCCD_L_lowerLimit, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tCCD_L_WR, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tCCD_L_WR_lowerLimit, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tCCD_L_WR2, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tCCD_L_WR2_lowerLimit, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tFAW, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tCCD_L_WTR, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tCCD_L_WTR_lowerLimit, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tCCD_S_WTR, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tCCD_S_WTR_lowerLimit, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tRTP, (s, e) => vm.Refresh());
            vm.XMPProfile.Bind(x => x.tRTP_lowerLimit, (s, e) => vm.Refresh());
        }
        private void BindNotifyPropertyChanged(EXPOEditorViewModel vm)
        {
            // Bind timings
            vm.EXPOProfile.Bind(x => x.MinCycleTime, (s, e) => vm.Refresh());
            vm.EXPOProfile.Bind(x => x.VDD, (s, e) => vm.Refresh());
            vm.EXPOProfile.Bind(x => x.VDDQ, (s, e) => vm.Refresh());
            vm.EXPOProfile.Bind(x => x.VPP, (s, e) => vm.Refresh());
            vm.EXPOProfile.Bind(x => x.tAA, (s, e) => vm.Refresh());
            vm.EXPOProfile.Bind(x => x.tRCD, (s, e) => vm.Refresh());
            vm.EXPOProfile.Bind(x => x.tRP, (s, e) => vm.Refresh());
            vm.EXPOProfile.Bind(x => x.tRAS, (s, e) => vm.Refresh());
            vm.EXPOProfile.Bind(x => x.tRC, (s, e) => vm.Refresh());
            vm.EXPOProfile.Bind(x => x.tWR, (s, e) => vm.Refresh());
            vm.EXPOProfile.Bind(x => x.tRFC1, (s, e) => vm.Refresh());
            vm.EXPOProfile.Bind(x => x.tRFC2, (s, e) => vm.Refresh());
            vm.EXPOProfile.Bind(x => x.tRFC, (s, e) => vm.Refresh());
        }
    }
}
