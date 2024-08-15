using Stylet;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace DDR5XMPEditor.DDR5SPD
{
    public class EXPO : PropertyChangedBase
    {
        public static readonly ushort EXPOOffset = 0x340;
        public static readonly ushort EXPOSize = 0x80;
        public static readonly ushort EXPOProfileSize = 0x28;
        public static readonly ushort EXPOHeaderSize = 0xA;
        public static readonly byte[] EXPOHeaderMagic = { 0x45, 0x58, 0x50, 0x4F };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]

        public unsafe struct RawEXPOHeader
        {
            public fixed byte magic[4];
            public byte revision; // Always 10?
            public byte enabledProfiles; // Profiles enabled 0011 0011
            public byte unk3; // Enhanced Timings?
            public byte zero_7;
            public byte zero_8;
            public byte zero_9;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct RawEXPOProfile
        {
            public byte vdd;
            public byte vddq;
            public byte vpp;
            public byte unk1;
            public fixed byte minCycleTime[2];
            public fixed byte tAA[2];
            public fixed byte tRCD[2];
            public fixed byte tRP[2];
            public fixed byte tRAS[2];
            public fixed byte tRC[2];
            public fixed byte tWR[2];
            public fixed byte tRFC1[2];
            public fixed byte tRFC2[2];
            public fixed byte tRFC[2];
            public fixed byte tRRD_L[2];
            public fixed byte tCCD_L[2];
            public fixed byte tCCD_L_WR[2];
            public fixed byte tCCD_L_WR2[2];
            public fixed byte tFAW[2];
            public fixed byte tCCD_L_WTR[2];
            public fixed byte tCCD_S_WTR[2];
            public fixed byte tRTP[2];
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct RawEXPO
        {
            public RawEXPOHeader header;
            public RawEXPOProfile profile1;
            public RawEXPOProfile profile2;
            public fixed byte filler[0x24];

            // Byte 0x7E-0x7F
            public fixed byte checksum[2];
        }

        public RawEXPOProfile rawEXPOProfile;
        ushort profileNo;

        public ushort VDD
        {
            get
            {
                return Utilities.ConvertByteToVoltageDDR5(rawEXPOProfile.vdd);
            }
            set
            {
                rawEXPOProfile.vdd = Utilities.ConvertVoltageToByteDDR5(value);
            }
        }

        public ushort VDDQ
        {
            get
            {
                return Utilities.ConvertByteToVoltageDDR5(rawEXPOProfile.vddq);
            }
            set
            {
                rawEXPOProfile.vddq = Utilities.ConvertVoltageToByteDDR5(value);
            }
        }

        public ushort VPP
        {
            get
            {
                return Utilities.ConvertByteToVoltageDDR5(rawEXPOProfile.vpp);
            }
            set
            {
                rawEXPOProfile.vpp = Utilities.ConvertVoltageToByteDDR5(value);
            }
        }
        public unsafe ushort MinCycleTime
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.minCycleTime[0], rawEXPOProfile.minCycleTime[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.minCycleTime[0], ref rawEXPOProfile.minCycleTime[1], value);
            }
        }
        public unsafe ushort tAA
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tAA[0], rawEXPOProfile.tAA[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tAA[0], ref rawEXPOProfile.tAA[1], value);
            }
        }
        public unsafe ushort tAATicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tAA, MinCycleTime);
            }
        }

        public unsafe ushort tRCD
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tRCD[0], rawEXPOProfile.tRCD[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tRCD[0], ref rawEXPOProfile.tRCD[1], value);
            }
        }

        public unsafe ushort tRCDTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tRCD, MinCycleTime);
            }
        }

        public unsafe ushort tRP
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tRP[0], rawEXPOProfile.tRP[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tRP[0], ref rawEXPOProfile.tRP[1], value);
            }
        }

        public unsafe ushort tRPTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tRP, MinCycleTime);
            }
        }

        public unsafe ushort tRAS
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tRAS[0], rawEXPOProfile.tRAS[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tRAS[0], ref rawEXPOProfile.tRAS[1], value);
            }
        }
        public unsafe ushort tRASTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tRAS, MinCycleTime);
            }
        }
        public unsafe ushort tRC
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tRC[0], rawEXPOProfile.tRC[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tRC[0], ref rawEXPOProfile.tRC[1], value);
            }
        }
        public unsafe ushort tRCTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tRC, MinCycleTime);
            }
        }
        public unsafe ushort tWR
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tWR[0], rawEXPOProfile.tWR[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tWR[0], ref rawEXPOProfile.tWR[1], value);
            }
        }
        public unsafe ushort tWRTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tWR, MinCycleTime);
            }

        }

        public unsafe ushort tRFC1
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tRFC1[0], rawEXPOProfile.tRFC1[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tRFC1[0], ref rawEXPOProfile.tRFC1[1], value);
            }
        }
        public unsafe ushort tRFC1Ticks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5((uint)(tRFC1 * 1000), MinCycleTime);
            }
        }
        public unsafe ushort tRFC2
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tRFC2[0], rawEXPOProfile.tRFC2[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tRFC2[0], ref rawEXPOProfile.tRFC2[1], value);
            }
        }
        public unsafe ushort tRFC2Ticks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5((uint)(tRFC2 * 1000), MinCycleTime);
            }
        }
        public unsafe ushort tRFC
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tRFC[0], rawEXPOProfile.tRFC[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tRFC[0], ref rawEXPOProfile.tRFC[1], value);
            }
        }
        public unsafe ushort tRFCTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5((uint)(tRFC * 1000), MinCycleTime);
            }
        }
        public byte[] GetBytes()
        {
            IntPtr ptr = IntPtr.Zero;
            byte[] bytes = null;
            try
            {
                var size = Marshal.SizeOf<RawEXPOProfile>();
                bytes = new byte[size];
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(rawEXPOProfile, ptr, true);
                Marshal.Copy(ptr, bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return bytes;
        }
        public static EXPO Parse(ushort profileNumber, byte[] bytes)
        {
            if (bytes.Length != EXPOProfileSize)
            {
                return null;
            }

            var handle_expo = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            EXPO expo = new EXPO
            {
                rawEXPOProfile = Marshal.PtrToStructure<RawEXPOProfile>(handle_expo.AddrOfPinnedObject()),
                profileNo = profileNumber
            };
            handle_expo.Free();
            return expo;
        }
        public unsafe ushort tRRD_L
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tRRD_L[0], rawEXPOProfile.tRRD_L[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tRRD_L[0], ref rawEXPOProfile.tRRD_L[1], value);
            }
        }
        public unsafe ushort tRRD_LTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tRRD_L, MinCycleTime);
            }
        }
        public unsafe ushort tCCD_L
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tCCD_L[0], rawEXPOProfile.tCCD_L[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tCCD_L[0], ref rawEXPOProfile.tCCD_L[1], value);
            }
        }
        public unsafe ushort tCCD_LTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tCCD_L, MinCycleTime);
            }
        }
        public unsafe ushort tCCD_L_WR
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tCCD_L_WR[0], rawEXPOProfile.tCCD_L_WR[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tCCD_L_WR[0], ref rawEXPOProfile.tCCD_L_WR[1], value);
            }
        }
        public unsafe ushort tCCD_L_WRTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tCCD_L_WR, MinCycleTime);
            }
        }
        public unsafe ushort tCCD_L_WR2
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tCCD_L_WR2[0], rawEXPOProfile.tCCD_L_WR2[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tCCD_L_WR2[0], ref rawEXPOProfile.tCCD_L_WR2[1], value);
            }
        }
        public unsafe ushort tCCD_L_WR2Ticks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tCCD_L_WR2, MinCycleTime);
            }
        }
        public unsafe ushort tFAW
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tFAW[0], rawEXPOProfile.tFAW[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tFAW[0], ref rawEXPOProfile.tFAW[1], value);
            }
        }
        public unsafe ushort tFAWTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tFAW, MinCycleTime);
            }
        }
        public unsafe ushort tCCD_L_WTR
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tCCD_L_WTR[0], rawEXPOProfile.tCCD_L_WTR[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tCCD_L_WTR[0], ref rawEXPOProfile.tCCD_L_WTR[1], value);
            }
        }
        public unsafe ushort tCCD_L_WTRTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tCCD_L_WTR, MinCycleTime);
            }
        }
        public unsafe ushort tCCD_S_WTR
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tCCD_S_WTR[0], rawEXPOProfile.tCCD_S_WTR[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tCCD_S_WTR[0], ref rawEXPOProfile.tCCD_S_WTR[1], value);
            }
        }
        public unsafe ushort tCCD_S_WTRTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tCCD_S_WTR, MinCycleTime);
            }
        }
        public unsafe ushort tRTP
        {
            get
            {
                return Utilities.ConvertBytes(rawEXPOProfile.tRTP[0], rawEXPOProfile.tRTP[1]);
            }

            set
            {
                Utilities.Convert16bitUnsignedInteger(ref rawEXPOProfile.tRTP[0], ref rawEXPOProfile.tRTP[1], value);
            }
        }
        public unsafe ushort tRTPTicks
        {
            get
            {
                return (ushort)Utilities.TimeToTicksDDR5(tRTP, MinCycleTime);
            }
        }
        public unsafe bool EnhancedTimings {
            get
            {
                return false;
            }
            set
            { 
            
            }
        }
    }
}
