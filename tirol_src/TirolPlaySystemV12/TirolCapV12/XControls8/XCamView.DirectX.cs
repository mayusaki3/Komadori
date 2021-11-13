using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace XControls
{
    partial class XCamView
    {
        #region BITMAP関連

        #region Struct定義

        #region BITMAPFILEHEADER

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BITMAPFILEHEADER
        {
            public ushort bfType;
            public uint bfSize;
            public ushort bfReserved1;
            public ushort bfReserved2;
            public uint bfOffBits;
        }

        #endregion

        #region BITMAPINFOHEADER

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct BITMAPINFOHEADER
        {
            public uint biSize;
            public int biWidth;
            public int biHeight;
            public ushort biPlanes;
            public ushort biBitCount;
            public uint biCompression;
            public uint biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public uint biClrUsed;
            public uint biClrImportant;
        };

        #endregion

        #endregion

        #endregion

        #region DirectShow定義

        #region 定数

        const int S_OK = 0;

        #endregion

        #region Enum定義

        #region CDef

        [Flags]
        public enum CDef
        {
            None = 0,
            ClassDefault = 0x0001,
            BypassClassManager = 0x0002,
            ClassLegacy = 0x0004,
            MeritAboveDoNotUse = 0x0008,
            DevmonCmgrDevice = 0x0010,
            DevmonDmo = 0x0020,
            DevmonPnpDevice = 0x0040,
            DevmonFilter = 0x0080,
            DevmonSelectiveMask = 0x00f0
        }

        #endregion

        #region FilterState

        public enum FilterState
        {
            Stopped,
            Paused,
            Running
        }

        #endregion

        #region PinDirection

        public enum PinDirection
        {
            Input,
            Output
        }

        #endregion

        #region AMRenderExFlags

        [Flags]
        public enum AMRenderExFlags
        {
            None = 0,
            RenderToExistingRenderers = 1
        }

        #endregion

        #region OABool

        public enum OABool
        {
            False = 0,
            True = -1
        }

        #endregion

        #region WindowStyle

        [Flags]
        public enum WindowStyle
        {
            Overlapped      = 0x00000000,
            Popup           = unchecked((int)0x80000000),
            Child           = 0x40000000,
            Minimize        = 0x20000000,
            Visible         = 0x10000000,
            Disabled        = 0x08000000,
            ClipSiblings    = 0x04000000,
            ClipChildren    = 0x02000000,
            Maximize        = 0x01000000,
            Caption         = 0x00C00000,
            Border          = 0x00800000,
            DlgFrame        = 0x00400000,
            VScroll         = 0x00200000,
            HScroll         = 0x00100000,
            SysMenu         = 0x00080000,
            ThickFrame      = 0x00040000,
            Group           = 0x00020000,
            TabStop         = 0x00010000,
            MinimizeBox     = 0x00020000,
            MaximizeBox     = 0x00010000
        }

        #endregion

        #region WindowStyleEx

        [Flags]
        public enum WindowStyleEx
        {
            DlgModalFrame   = 0x00000001,
            NoParentNotify  = 0x00000004,
            Topmost         = 0x00000008,
            AcceptFiles     = 0x00000010,
            Transparent     = 0x00000020,
            MDIChild        = 0x00000040,
            ToolWindow      = 0x00000080,
            WindowEdge      = 0x00000100,
            ClientEdge      = 0x00000200,
            ContextHelp     = 0x00000400,
            Right           = 0x00001000,
            Left            = 0x00000000,
            RTLReading      = 0x00002000,
            LTRReading      = 0x00000000,
            LeftScrollBar   = 0x00004000,
            RightScrollBar  = 0x00000000,
            ControlParent   = 0x00010000,
            StaticEdge      = 0x00020000,
            APPWindow       = 0x00040000,
            Layered         = 0x00080000,
            NoInheritLayout = 0x00100000,
            LayoutRTL       = 0x00400000,
            Composited      = 0x02000000,
            NoActivate      = 0x08000000
        }

        #endregion

        #region WindowState

        public enum WindowState
        {
            Hide = 0,
            Normal,
            ShowMinimized,
            ShowMaximized,
            ShowNoActivate,
            Show,
            Minimize,
            ShowMinNoActive,
            ShowNA,
            Restore,
            ShowDefault,
            ForceMinimize
        }
        
        #endregion

        #endregion

        #region Struct定義

        #region FilterInfo

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FilterInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string achName;
            [MarshalAs(UnmanagedType.Interface)]
            public IFilterGraph pGraph;
        }

        #endregion

        #region PinInfo

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct PinInfo
        {
            [MarshalAs(UnmanagedType.Interface)]
            public IBaseFilter filter;
            public PinDirection dir;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string name;
        }

        #endregion

        #region DsRECT

        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public struct DsRECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        #endregion

        #region DsBITMAPINFOHEADER

        [StructLayout(LayoutKind.Sequential, Pack = 2), ComVisible(false)]
        public struct DsBITMAPINFOHEADER
        {
            public int Size;
            public int Width;
            public int Height;
            public short Planes;
            public short BitCount;
            public int Compression;
            public int ImageSize;
            public int XPelsPerMeter;
            public int YPelsPerMeter;
            public int ClrUsed;
            public int ClrImportant;
        }

        #endregion

        #endregion

        #region Interface定義

        #region IPersist

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("0000010c-0000-0000-C000-000000000046"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IPersist
        {
            [PreserveSig]
            int GetClassID([Out] out Guid pClassID);
        }

        #endregion

        #region IReferenceClock

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("56a86897-0ad4-11ce-b03a-0020af0ba770"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IReferenceClock
        {
            [PreserveSig]
            int GetTime([Out] out long pTime);

            [PreserveSig]
            int AdviseTime(
                [In] long baseTime,
                [In] long streamTime,
                [In] IntPtr hEvent,
                [Out] out int pdwAdviseCookie
                );

            [PreserveSig]
            int AdvisePeriodic(
                [In] long startTime,
                [In] long periodTime,
                [In] IntPtr hSemaphore,
                [Out] out int pdwAdviseCookie
                );

            [PreserveSig]
            int Unadvise([In] int dwAdviseCookie);
        }

        #endregion

        #region IMediaFilter

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("56a86899-0ad4-11ce-b03a-0020af0ba770"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IMediaFilter : IPersist
        {
            [PreserveSig]
            new int GetClassID(
                [Out] out Guid pClassID);

            [PreserveSig]
            int Stop();

            [PreserveSig]
            int Pause();

            [PreserveSig]
            int Run([In] long tStart);

            [PreserveSig]
            int GetState(
                [In] int dwMilliSecsTimeout,
                [Out] out FilterState filtState
                );

            [PreserveSig]
            int SetSyncSource([In] IReferenceClock pClock);

            [PreserveSig]
            int GetSyncSource([Out] out IReferenceClock pClock);
        }

        #endregion

        #region IEnumPins

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("56a86892-0ad4-11ce-b03a-0020af0ba770"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IEnumPins
        {
            [PreserveSig]
            int Next(
                [In] int cPins,
                [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IPin[] ppPins,
                [In] IntPtr pcFetched
                );

            [PreserveSig]
            int Skip([In] int cPins);

            [PreserveSig]
            int Reset();

            [PreserveSig]
            int Clone([Out] out IEnumPins ppEnum);
        }

        #endregion

        #region IEnumMediaTypes

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("89c31040-846b-11ce-97d3-00aa0055595a"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IEnumMediaTypes
        {
            [PreserveSig]
            int Next(
                [In] int cMediaTypes,
                [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(EMTMarshaler), SizeParamIndex = 0)] AMMediaType[] ppMediaTypes,
                [In] IntPtr pcFetched
                );

            [PreserveSig]
            int Skip([In] int cMediaTypes);

            [PreserveSig]
            int Reset();

            [PreserveSig]
            int Clone([Out] out IEnumMediaTypes ppEnum);
        }

        #endregion

        #region IPin

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("56a86891-0ad4-11ce-b03a-0020af0ba770"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IPin
        {
            [PreserveSig]
            int Connect(
                [In] IPin pReceivePin,
                [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
                );

            [PreserveSig]
            int ReceiveConnection(
                [In] IPin pReceivePin,
                [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
                );

            [PreserveSig]
            int Disconnect();

            [PreserveSig]
            int ConnectedTo(
                [Out] out IPin ppPin);

            [PreserveSig]
            int ConnectionMediaType(
                [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

            [PreserveSig]
            int QueryPinInfo([Out] out PinInfo pInfo);

            [PreserveSig]
            int QueryDirection(out PinDirection pPinDir);

            [PreserveSig]
            int QueryId([Out, MarshalAs(UnmanagedType.LPWStr)] out string Id);

            [PreserveSig]
            int QueryAccept([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);

            [PreserveSig]
            int EnumMediaTypes([Out] out IEnumMediaTypes ppEnum);

            [PreserveSig]
            int QueryInternalConnections(
                [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] IPin[] ppPins,
                [In, Out] ref int nPin
                );

            [PreserveSig]
            int EndOfStream();

            [PreserveSig]
            int BeginFlush();

            [PreserveSig]
            int EndFlush();

            [PreserveSig]
            int NewSegment(
                [In] long tStart,
                [In] long tStop,
                [In] double dRate
                );
        }

        #endregion

        #region IBaseFilter

        [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
        Guid("56a86895-0ad4-11ce-b03a-0020af0ba770"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IBaseFilter : IMediaFilter
        {
            [PreserveSig]
            new int GetClassID(
                [Out] out Guid pClassID);

            [PreserveSig]
            new int Stop();

            [PreserveSig]
            new int Pause();

            [PreserveSig]
            new int Run(long tStart);

            [PreserveSig]
            new int GetState([In] int dwMilliSecsTimeout, [Out] out FilterState filtState);

            [PreserveSig]
            new int SetSyncSource([In] IReferenceClock pClock);

            [PreserveSig]
            new int GetSyncSource([Out] out IReferenceClock pClock);

            [PreserveSig]
            int EnumPins([Out] out IEnumPins ppEnum);

            [PreserveSig]
            int FindPin(
                [In, MarshalAs(UnmanagedType.LPWStr)] string Id,
                [Out] out IPin ppPin
                );

            [PreserveSig]
            int QueryFilterInfo([Out] out FilterInfo pInfo);

            [PreserveSig]
            int JoinFilterGraph(
                [In] IFilterGraph pGraph,
                [In, MarshalAs(UnmanagedType.LPWStr)] string pName
                );

            [PreserveSig]
            int QueryVendorInfo([Out, MarshalAs(UnmanagedType.LPWStr)] out string pVendorInfo);
        }

        #endregion

        #region ICreateDevEnum

        const string CLSID_SystemDeviceEnum = "62BE5D10-60EB-11d0-BD3B-00A0C911CE86";

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface ICreateDevEnum
        {
            [PreserveSig]
            int CreateClassEnumerator(
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid pType,
                [Out] out IEnumMoniker ppEnumMoniker,
                [In] CDef dwFlags
                );
        }

        #endregion

        #region IErrorLog

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("3127CA40-446E-11CE-8135-00AA004BB851"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IErrorLog
        {
            [PreserveSig]
            int AddError(
                [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
                [In] System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo
                );
        }

        #endregion

        #region IEnumFilters

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("56a86893-0ad4-11ce-b03a-0020af0ba770"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IEnumFilters
        {
            [PreserveSig]
            int Next(
                [In] int cFilters,
                [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IBaseFilter[] ppFilter,
                [In] IntPtr pcFetched
                );

            [PreserveSig]
            int Skip([In] int cFilters);

            [PreserveSig]
            int Reset();

            [PreserveSig]
            int Clone([Out] out IEnumFilters ppEnum);
        }

        #endregion

        #region IFilterGraph

        const string CLSID_FilterGraph = "e436ebb3-524f-11ce-9f53-0020af0ba770";

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("56a8689f-0ad4-11ce-b03a-0020af0ba770"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IFilterGraph
        {
            [PreserveSig]
            int AddFilter(
                [In] IBaseFilter pFilter,
                [In, MarshalAs(UnmanagedType.LPWStr)] string pName
                );

            [PreserveSig]
            int RemoveFilter([In] IBaseFilter pFilter);

            [PreserveSig]
            int EnumFilters([Out] out IEnumFilters ppEnum);

            [PreserveSig]
            int FindFilterByName(
                [In, MarshalAs(UnmanagedType.LPWStr)] string pName,
                [Out] out IBaseFilter ppFilter
                );

            [PreserveSig]
            int ConnectDirect(
                [In] IPin ppinOut,
                [In] IPin ppinIn,
                [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
                );

            [PreserveSig]
            [Obsolete("このメソッドは現在使われていない。このメソッドの代わりに IFilterGraph2::ReconnectEx メソッドを使うこと。")]
            int Reconnect([In] IPin ppin);

            [PreserveSig]
            int Disconnect([In] IPin ppin);

            [PreserveSig]
            int SetDefaultSyncSource();
        }

        #endregion

        #region IGraphBuilder

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("56a868a9-0ad4-11ce-b03a-0020af0ba770"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IGraphBuilder : IFilterGraph
        {
            [PreserveSig]
            new int AddFilter(
                [In] IBaseFilter pFilter,
                [In, MarshalAs(UnmanagedType.LPWStr)] string pName
                );

            [PreserveSig]
            new int RemoveFilter([In] IBaseFilter pFilter);

            [PreserveSig]
            new int EnumFilters([Out] out IEnumFilters ppEnum);

            [PreserveSig]
            new int FindFilterByName(
                [In, MarshalAs(UnmanagedType.LPWStr)] string pName,
                [Out] out IBaseFilter ppFilter
                );

            [PreserveSig]
            new int ConnectDirect(
                [In] IPin ppinOut,
                [In] IPin ppinIn,
                [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
                );

            [PreserveSig]
            new int Reconnect([In] IPin ppin);

            [PreserveSig]
            new int Disconnect([In] IPin ppin);

            [PreserveSig]
            new int SetDefaultSyncSource();

            [PreserveSig]
            int Connect(
                [In] IPin ppinOut,
                [In] IPin ppinIn
                );

            [PreserveSig]
            int Render([In] IPin ppinOut);

            [PreserveSig]
            int RenderFile(
                [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFile,
                [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrPlayList
                );

            [PreserveSig]
            int AddSourceFilter(
                [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFileName,
                [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFilterName,
                [Out] out IBaseFilter ppFilter
                );

            [PreserveSig]
            int SetLogFile(IntPtr hFile);

            [PreserveSig]
            int Abort();

            [PreserveSig]
            int ShouldOperationContinue();
        }

        #endregion

        #region IFilterGraph2

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("36b73882-c2c8-11cf-8b46-00805f6cef60"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IFilterGraph2 : IGraphBuilder
        {
            [PreserveSig]
            new int AddFilter(
                [In] IBaseFilter pFilter,
                [In, MarshalAs(UnmanagedType.LPWStr)] string pName
                );

            [PreserveSig]
            new int RemoveFilter([In] IBaseFilter pFilter);

            [PreserveSig]
            new int EnumFilters([Out] out IEnumFilters ppEnum);

            [PreserveSig]
            new int FindFilterByName(
                [In, MarshalAs(UnmanagedType.LPWStr)] string pName,
                [Out] out IBaseFilter ppFilter
                );

            [PreserveSig]
            new int ConnectDirect(
                [In] IPin ppinOut,
                [In] IPin ppinIn,
                [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
                );

            [PreserveSig]
            new int Reconnect([In] IPin ppin);

            [PreserveSig]
            new int Disconnect([In] IPin ppin);

            [PreserveSig]
            new int SetDefaultSyncSource();

            [PreserveSig]
            new int Connect(
                [In] IPin ppinOut,
                [In] IPin ppinIn
                );

            [PreserveSig]
            new int Render([In] IPin ppinOut);

            [PreserveSig]
            new int RenderFile(
                [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFile,
                [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrPlayList
                );

            [PreserveSig]
            new int AddSourceFilter(
                [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFileName,
                [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFilterName,
                [Out] out IBaseFilter ppFilter
                );

            [PreserveSig]
            new int SetLogFile(IntPtr hFile);

            [PreserveSig]
            new int Abort();

            [PreserveSig]
            new int ShouldOperationContinue();

            [PreserveSig]
            int AddSourceFilterForMoniker(
                [In] IMoniker pMoniker,
                [In] IBindCtx pCtx,
                [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFilterName,
                [Out] out IBaseFilter ppFilter
                );

            [PreserveSig]
            int ReconnectEx(
                [In] IPin ppin,
                [In] AMMediaType pmt
                );

            [PreserveSig]
            int RenderEx(
                [In] IPin pPinOut,
                [In] AMRenderExFlags dwFlags,
                [In] IntPtr pvContext
                );
        }

        #endregion

        #region IPropertyBag

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("55272A00-42CB-11CE-8135-00AA004BB851"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IPropertyBag
        {
            [PreserveSig]
            int Read(
                [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
                [Out, MarshalAs(UnmanagedType.Struct)] out object pVar,
                [In] IErrorLog pErrorLog
                );

            [PreserveSig]
            int Write(
                [In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName,
                [In, MarshalAs(UnmanagedType.Struct)] ref object pVar
                );
        }

        #endregion

        #region IMediaControl

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("56a868b1-0ad4-11ce-b03a-0020af0ba770"),
            InterfaceType(ComInterfaceType.InterfaceIsDual)
        ]
        public interface IMediaControl
        {
            [PreserveSig]
            int Run();

            [PreserveSig]
            int Pause();

            [PreserveSig]
            int Stop();

            [PreserveSig]
            int GetState([In] int msTimeout, [Out] out FilterState pfs);

            [PreserveSig]
            int RenderFile([In, MarshalAs(UnmanagedType.BStr)] string strFilename);

            [PreserveSig,Obsolete()]
            int AddSourceFilter(
                [In, MarshalAs(UnmanagedType.BStr)] string strFilename,
                [Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk
                );

            [PreserveSig, Obsolete()]
            int get_FilterCollection([Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);

            [PreserveSig, Obsolete()]
            int get_RegFilterCollection([Out, MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);

            [PreserveSig]
            int StopWhenReady();
        }

        #endregion

        #region IVideoWindow

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("56a868b4-0ad4-11ce-b03a-0020af0ba770"),
            InterfaceType(ComInterfaceType.InterfaceIsDual)
        ]
        public interface IVideoWindow
        {
            [PreserveSig]
            int put_Caption([In, MarshalAs(UnmanagedType.BStr)] string caption);

            [PreserveSig]
            int get_Caption([Out, MarshalAs(UnmanagedType.BStr)] out string caption);

            [PreserveSig]
            int put_WindowStyle([In] WindowStyle windowStyle);

            [PreserveSig]
            int get_WindowStyle([Out] out WindowStyle windowStyle);

            [PreserveSig]
            int put_WindowStyleEx([In] WindowStyleEx windowStyleEx);

            [PreserveSig]
            int get_WindowStyleEx([Out] out WindowStyleEx windowStyleEx);

            [PreserveSig]
            int put_AutoShow([In] OABool autoShow);

            [PreserveSig]
            int get_AutoShow([Out] out OABool autoShow);

            [PreserveSig]
            int put_WindowState([In] WindowState windowState);

            [PreserveSig]
            int get_WindowState([Out] out WindowState windowState);

            [PreserveSig]
            int put_BackgroundPalette([In] OABool backgroundPalette);

            [PreserveSig]
            int get_BackgroundPalette([Out] out OABool backgroundPalette);

            [PreserveSig]
            int put_Visible([In] OABool visible);

            [PreserveSig]
            int get_Visible([Out] out OABool visible);

            [PreserveSig]
            int put_Left([In] int left);

            [PreserveSig]
            int get_Left([Out] out int left);

            [PreserveSig]
            int put_Width([In] int width);

            [PreserveSig]
            int get_Width([Out] out int width);

            [PreserveSig]
            int put_Top([In] int top);

            [PreserveSig]
            int get_Top([Out] out int top);

            [PreserveSig]
            int put_Height([In] int height);

            [PreserveSig]
            int get_Height([Out] out int height);

            [PreserveSig]
            int put_Owner([In] IntPtr owner);

            [PreserveSig]
            int get_Owner([Out] out IntPtr owner);

            [PreserveSig]
            int put_MessageDrain([In] IntPtr drain);

            [PreserveSig]
            int get_MessageDrain([Out] out IntPtr drain);

            [PreserveSig]
            int get_BorderColor([Out] out int color);

            [PreserveSig]
            int put_BorderColor([In] int color);

            [PreserveSig]
            int get_FullScreenMode([Out] out OABool fullScreenMode);

            [PreserveSig]
            int put_FullScreenMode([In] OABool fullScreenMode);

            [PreserveSig]
            int SetWindowForeground([In] OABool focus);

            [PreserveSig]
            int NotifyOwnerMessage(
                [In] IntPtr hwnd,
                [In] int msg,
                [In] IntPtr wParam,
                [In] IntPtr lParam
                );

            [PreserveSig]
            int SetWindowPosition(
                [In] int left,
                [In] int top,
                [In] int width,
                [In] int height
                );

            [PreserveSig]
            int GetWindowPosition(
                [Out] out int left,
                [Out] out int top,
                [Out] out int width,
                [Out] out int height
                );

            [PreserveSig]
            int GetMinIdealImageSize(
                [Out] out int width,
                [Out] out int height
                );

            [PreserveSig]
            int GetMaxIdealImageSize(
                [Out] out int width,
                [Out] out int height
                );

            [PreserveSig]
            int GetRestorePosition(
                [Out] out int left,
                [Out] out int top,
                [Out] out int width,
                [Out] out int height
                );

            [PreserveSig]
            int HideCursor([In] OABool hideCursor);

            [PreserveSig]
            int IsCursorHidden([Out] out OABool hideCursor);
        }

        #endregion

        #region IBasicVideo

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("56a868b5-0ad4-11ce-b03a-0020af0ba770"),
            InterfaceType(ComInterfaceType.InterfaceIsDual)
        ]
        public interface IBasicVideo
        {
            [PreserveSig]
            int get_AvgTimePerFrame([Out] out double pAvgTimePerFrame);

            [PreserveSig]
            int get_BitRate([Out] out int pBitRate);

            [PreserveSig]
            int get_BitErrorRate([Out] out int pBitRate);

            [PreserveSig]
            int get_VideoWidth([Out] out int pVideoWidth);

            [PreserveSig]
            int get_VideoHeight([Out] out int pVideoHeight);

            [PreserveSig]
            int put_SourceLeft([In] int SourceLeft);

            [PreserveSig]
            int get_SourceLeft([Out] out int pSourceLeft);

            [PreserveSig]
            int put_SourceWidth([In] int SourceWidth);

            [PreserveSig]
            int get_SourceWidth([Out] out int pSourceWidth);

            [PreserveSig]
            int put_SourceTop([In] int SourceTop);

            [PreserveSig]
            int get_SourceTop([Out] out int pSourceTop);

            [PreserveSig]
            int put_SourceHeight([In] int SourceHeight);

            [PreserveSig]
            int get_SourceHeight([Out] out int pSourceHeight);

            [PreserveSig]
            int put_DestinationLeft([In] int DestinationLeft);

            [PreserveSig]
            int get_DestinationLeft([Out] out int pDestinationLeft);

            [PreserveSig]
            int put_DestinationWidth([In] int DestinationWidth);

            [PreserveSig]
            int get_DestinationWidth([Out] out int pDestinationWidth);

            [PreserveSig]
            int put_DestinationTop([In] int DestinationTop);

            [PreserveSig]
            int get_DestinationTop([Out] out int pDestinationTop);

            [PreserveSig]
            int put_DestinationHeight([In] int DestinationHeight);

            [PreserveSig]
            int get_DestinationHeight([Out] out int pDestinationHeight);

            [PreserveSig]
            int SetSourcePosition(
                [In] int left,
                [In] int top,
                [In] int width,
                [In] int height
                );

            [PreserveSig]
            int GetSourcePosition(
                [Out] out int left,
                [Out] out int top,
                [Out] out int width,
                [Out] out int height
                );

            [PreserveSig]
            int SetDefaultSourcePosition();

            [PreserveSig]
            int SetDestinationPosition(
                [In] int left,
                [In] int top,
                [In] int width,
                [In] int height
                );

            [PreserveSig]
            int GetDestinationPosition(
                [Out] out int left,
                [Out] out int top,
                [Out] out int width,
                [Out] out int height
                );

            [PreserveSig]
            int SetDefaultDestinationPosition();

            [PreserveSig]
            int GetVideoSize(
                [Out] out int pWidth,
                [Out] out int pHeight
                );

            [PreserveSig]
            int GetVideoPaletteEntries(
                [In] int StartIndex,
                [In] int Entries,
                [Out] out int pRetrieved,
                [Out] out int[] pPalette
                );

            [PreserveSig]
            int GetCurrentImage(
                [In, Out] ref int pBufferSize,
                [Out] IntPtr pDIBImage
                );

            [PreserveSig]
            int IsUsingDefaultSource();

            [PreserveSig]
            int IsUsingDefaultDestination();
        }

        #endregion

        #region IBasicVideo2

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("329bb360-f6ea-11d1-9038-00a0c9697298"),
            InterfaceType(ComInterfaceType.InterfaceIsDual)
        ]
        public interface IBasicVideo2 : IBasicVideo
        {
            #region IBasicVideo Methods

            [PreserveSig]
            new int get_AvgTimePerFrame([Out] out double pAvgTimePerFrame);

            [PreserveSig]
            new int get_BitRate([Out] out int pBitRate);

            [PreserveSig]
            new int get_BitErrorRate([Out] out int pBitRate);

            [PreserveSig]
            new int get_VideoWidth([Out] out int pVideoWidth);

            [PreserveSig]
            new int get_VideoHeight([Out] out int pVideoHeight);

            [PreserveSig]
            new int put_SourceLeft([In] int SourceLeft);

            [PreserveSig]
            new int get_SourceLeft([Out] out int pSourceLeft);

            [PreserveSig]
            new int put_SourceWidth([In] int SourceWidth);

            [PreserveSig]
            new int get_SourceWidth([Out] out int pSourceWidth);

            [PreserveSig]
            new int put_SourceTop([In] int SourceTop);

            [PreserveSig]
            new int get_SourceTop([Out] out int pSourceTop);

            [PreserveSig]
            new int put_SourceHeight([In] int SourceHeight);

            [PreserveSig]
            new int get_SourceHeight([Out] out int pSourceHeight);

            [PreserveSig]
            new int put_DestinationLeft([In] int DestinationLeft);

            [PreserveSig]
            new int get_DestinationLeft([Out] out int pDestinationLeft);

            [PreserveSig]
            new int put_DestinationWidth([In] int DestinationWidth);

            [PreserveSig]
            new int get_DestinationWidth([Out] out int pDestinationWidth);

            [PreserveSig]
            new int put_DestinationTop([In] int DestinationTop);

            [PreserveSig]
            new int get_DestinationTop([Out] out int pDestinationTop);

            [PreserveSig]
            new int put_DestinationHeight([In] int DestinationHeight);

            [PreserveSig]
            new int get_DestinationHeight([Out] out int pDestinationHeight);

            [PreserveSig]
            new int SetSourcePosition(
                [In] int left,
                [In] int top,
                [In] int width,
                [In] int height
                );

            [PreserveSig]
            new int GetSourcePosition(
                [Out] out int left,
                [Out] out int top,
                [Out] out int width,
                [Out] out int height
                );

            [PreserveSig]
            new int SetDefaultSourcePosition();

            [PreserveSig]
            new int SetDestinationPosition(
                [In] int left,
                [In] int top,
                [In] int width,
                [In] int height
                );

            [PreserveSig]
            new int GetDestinationPosition(
                [Out] out int left,
                [Out] out int top,
                [Out] out int width,
                [Out] out int height
                );

            [PreserveSig]
            new int SetDefaultDestinationPosition();

            [PreserveSig]
            new int GetVideoSize(
                [Out] out int pWidth,
                [Out] out int pHeight
                );

            [PreserveSig]
            new int GetVideoPaletteEntries(
                [In] int StartIndex,
                [In] int Entries,
                [Out] out int pRetrieved,
                [Out] out int[] pPalette
                );

            [PreserveSig]
            new int GetCurrentImage(
                [In, Out] ref int pBufferSize,
                [Out] IntPtr pDIBImage
                );

            [PreserveSig]
            new int IsUsingDefaultSource();

            [PreserveSig]
            new int IsUsingDefaultDestination();

            #endregion

            [PreserveSig]
            int GetPreferredAspectRatio(
                [Out] out int plAspectX,
                [Out] out int plAspectY
                );
        }

        #endregion

        #region ISampleGrabber

        const string CLSID_SampleGrabber = "c1f400a0-3f08-11d3-9f0b-006008039e37";

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("6B652FFF-11FE-4fce-92AD-0266B5D7C78F"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface ISampleGrabber
        {
            [PreserveSig]
            int SetOneShot(
                [In, MarshalAs(UnmanagedType.Bool)]	bool OneShot
                );

            [PreserveSig]
            int SetMediaType(
                [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
                );

            [PreserveSig]
            int GetConnectedMediaType(
                [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt
                );

            [PreserveSig]
                int SetBufferSamples(
                    [In, MarshalAs(UnmanagedType.Bool)] bool BufferThem
                );

            [PreserveSig]
                int GetCurrentBuffer(ref int pBufferSize, IntPtr pBuffer);

            [PreserveSig]
                int GetCurrentSample(IntPtr ppSample);

            [PreserveSig]
                int SetCallback(ISampleGrabberCB pCallback, int WhichMethodToCallback);
        }

        #endregion

        #region ISampleGrabberCB

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("0579154A-2B53-4994-B0D0-E773148EFF85"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface ISampleGrabberCB
        {
            [PreserveSig]
                int SampleCB(double SampleTime, IMediaSample pSample);

            [PreserveSig]
                int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen);
        }

        #endregion

        #region IMediaSample

        [
            ComImport, System.Security.SuppressUnmanagedCodeSecurity,
            Guid("56a8689a-0ad4-11ce-b03a-0020af0ba770"),
            InterfaceType(ComInterfaceType.InterfaceIsIUnknown)
        ]
        public interface IMediaSample
        {
            [PreserveSig]
                int GetPointer(out IntPtr ppBuffer);
            
            [PreserveSig]
                int GetSize();

            [PreserveSig]
                int GetTime(out long pTimeStart, out long pTimeEnd);

            [PreserveSig]
                int SetTime(
                    [In, MarshalAs(UnmanagedType.LPStruct)] DsOptInt64 pTimeStart,
                    [In, MarshalAs(UnmanagedType.LPStruct)] DsOptInt64 pTimeEnd
                );

            [PreserveSig]
                int IsSyncPoint();
            [PreserveSig]
                int SetSyncPoint(
                    [In, MarshalAs(UnmanagedType.Bool)] bool bIsSyncPoint
                );

            [PreserveSig]
                int IsPreroll();
            
            [PreserveSig]
                int SetPreroll(
                    [In, MarshalAs(UnmanagedType.Bool)] bool bIsPreroll
                );

            [PreserveSig]
                int GetActualDataLength();
            
            [PreserveSig]
                int SetActualDataLength(int len);

            [PreserveSig]
                int GetMediaType(
                    [Out, MarshalAs(UnmanagedType.LPStruct)] out AMMediaType ppMediaType
                );

            [PreserveSig]
                int SetMediaType(
                    [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pMediaType
                );

            [PreserveSig]
                int IsDiscontinuity();
            
            [PreserveSig]
                int SetDiscontinuity(
                    [In, MarshalAs(UnmanagedType.Bool)] bool bDiscontinuity
                );

            [PreserveSig]
                int GetMediaTime(out long pTimeStart, out long pTimeEnd);

            [PreserveSig]
                int SetMediaTime(
                    [In, MarshalAs(UnmanagedType.LPStruct)] DsOptInt64 pTimeStart,
                    [In, MarshalAs(UnmanagedType.LPStruct)] DsOptInt64 pTimeEnd
                );
        }

        #endregion

        #endregion

        #region Class定義

        #region FilterCategory

        sealed public class FilterCategory
        {
            private FilterCategory()
            {
            }

            // Microsoft DirectX 9.0フィルタ カテゴリ

            // 以下、uuids.hより
            public static readonly Guid AudioInputDeviceCategory= new Guid("33D9A762-90C8-11d0-BD43-00A0C911CE86");
            public static readonly Guid AudioCompressorCategory = new Guid("33D9A761-90C8-11d0-BD43-00A0C911CE86");
            public static readonly Guid AudioRendererCategory   = new Guid("E0F158E1-CB04-11d0-BD4E-00A0C911CE86");
            public static readonly Guid DeviceControlCategory   = new Guid("CC7BFB46-F175-11d1-A392-00E0291F3959");
            public static readonly Guid LegacyAmFilterCategory  = new Guid("083863F1-70DE-11d0-BD40-00A0C911CE86");
            public static readonly Guid TransmitCategory        = new Guid("CC7BFB41-F175-11d1-A392-00E0291F3959");
            public static readonly Guid MidiRendererCategory    = new Guid("4EFE2452-168A-11d1-BC76-00C04FB9453B");
            public static readonly Guid VideoInputDeviceCategory= new Guid("860BB310-5D01-11d0-BD3B-00A0C911CE86");
            public static readonly Guid VideoCompressorCategory = new Guid("33D9A760-90C8-11d0-BD43-00A0C911CE86");
            public static readonly Guid VideoEffects1Category   = new Guid("CC7BFB42-F175-11d1-A392-00E0291F3959");
            public static readonly Guid VideoEffects2Category   = new Guid("CC7BFB43-F175-11d1-A392-00E0291F3959");
            public static readonly Guid AMKSCategoryCapture     = new Guid("65E8773D-8F56-11D0-A3B9-00A0C9223196");
            public static readonly Guid AMKSCategoryCrossbar    = new Guid("a799a801-a46d-11d0-a18c-00a02401dcd4");
            public static readonly Guid AMKSCategoryRender      = new Guid("65E8773E-8F56-11D0-A3B9-00A0C9223196");
            public static readonly Guid AMKSCategorySplitter    = new Guid("0A4252A0-7E70-11D0-A5D6-28DB04C10000");
            public static readonly Guid AMKSCategoryTVAudio     = new Guid("a799a802-a46d-11d0-a18c-00a02401dcd4");
            public static readonly Guid AMKSCategoryTVTuner     = new Guid("a799a800-a46d-11d0-a18c-00a02401dcd4");
            public static readonly Guid AMKSCategoryVBICodec    = new Guid("07dad660-22f1-11d1-a9f4-00c04fbbde8f");
            public static readonly Guid ActiveMovieCategories   = new Guid("DA4E3DA0-D07D-11d0-BD50-00A0C911CE86");

            // 以下、Ks.hより
            public static readonly Guid KSCategoryCommunicationsTransform   = new Guid("CF1DDA2C-9743-11D0-A3EE-00A0C9223196");
            public static readonly Guid KSCategoryDataTransform             = new Guid("2EB07EA0-7E70-11D0-A5D6-28DB04C10000");
            public static readonly Guid KSCategoryInterfaceTransform        = new Guid("CF1DDA2D-9743-11D0-A3EE-00A0C9223196");
            public static readonly Guid KSCategoryMixer                     = new Guid("AD809C00-7B88-11D0-A5D6-28DB04C10000");
            
            // 以下、ksmedia.hより
            public static readonly Guid KSCategoryAudioDevice               = new Guid("FBF6F530-07B9-11D2-A71E-0000F8004788");

            // 以下、Bdamedia.hより
            public static readonly Guid CPCAFiltersCategory                 = new Guid("C4C4C4FC-0049-4E2B-98FB-9537F6CE516D");
            public static readonly Guid KSCategoryBDANetworkProvider        = new Guid("71985f4b-1ca1-11d3-9cc8-00c04f7971e0");
            public static readonly Guid KSCategoryBDAReceiverComponent      = new Guid("FD0A5AF4-B41D-11d2-9c95-00c04f7971e0");
            //public static readonly Guid KSCategoryIPSink                    = new Guid("????????-????-????-????-????????????");
            //public static readonly Guid KSCategoryBDANetworkTuner           = new Guid("????????-????-????-????-????????????");
            //public static readonly Guid KSCategoryBDATransportInformation   = new Guid("????????-????-????-????-????????????");
        }

        #endregion

        #region AMMediaType

        [StructLayout(LayoutKind.Sequential)]
        public class AMMediaType
        {
            public Guid majorType;
            public Guid subType;
            [MarshalAs(UnmanagedType.Bool)]
                public bool fixedSizeSamples;
            [MarshalAs(UnmanagedType.Bool)]
                public bool temporalCompression;
            public int sampleSize;
            public Guid formatType;
            public IntPtr unkPtr;
            public int formatSize;
            public IntPtr formatPtr;
        }

        #endregion

        #region MediaType

        public class MediaType
        {
            public static readonly Guid Video       = new Guid("73646976-0000-0010-8000-00aa00389b71");
            //public static readonly Guid Interleaved = new Guid("73766169-0000-0010-8000-00aa00389b71");
            //public static readonly Guid Audio       = new Guid("73647561-0000-0010-8000-00aa00389b71");
            //public static readonly Guid Text        = new Guid("73747874-0000-0010-8000-00aa00389b71");
            //public static readonly Guid Stream      = new Guid("e436eb83-524f-11ce-9f53-0020af0ba770");
        }

        #endregion

        #region MediaSubType

        public class MediaSubType
        {
            //public static readonly Guid YUYV    = new Guid("56595559-0000-0010-8000-00aa00389b71");
            //public static readonly Guid IYUV    = new Guid("56555949-0000-0010-8000-00aa00389b71");
            //public static readonly Guid DVSD    = new Guid("44535644-0000-0010-8000-00aa00389b71");
            //public static readonly Guid RGB1    = new Guid("e436eb78-524f-11ce-9f53-0020af0ba770");
            //public static readonly Guid RGB4    = new Guid("e436eb79-524f-11ce-9f53-0020af0ba770");
            //public static readonly Guid RGB8    = new Guid("e436eb7a-524f-11ce-9f53-0020af0ba770");
            //public static readonly Guid RGB565  = new Guid("e436eb7b-524f-11ce-9f53-0020af0ba770");
            //public static readonly Guid RGB555  = new Guid("e436eb7c-524f-11ce-9f53-0020af0ba770");
            public static readonly Guid RGB24   = new Guid("e436eb7d-524f-11ce-9f53-0020af0ba770");
            public static readonly Guid RGB32   = new Guid("e436eb7e-524f-11ce-9f53-0020af0ba770");
            //public static readonly Guid Avi     = new Guid("e436eb88-524f-11ce-9f53-0020af0ba770");
            //public static readonly Guid Asf     = new Guid("3db80f90-9412-11d1-aded-0000f8754b99");
        }

        #endregion

        #region FormatType

        public class FormatType
        {
            public static readonly Guid None        = new Guid("0F6417D6-c318-11d0-a43f-00a0c9223196");
            public static readonly Guid VideoInfo   = new Guid("05589f80-c356-11ce-bf01-00aa0055595a");
            //public static readonly Guid VideoInfo2  = new Guid("f72a76A0-eb0a-11d0-ace4-0000c0cc16ba");
            //public static readonly Guid WaveEx      = new Guid("05589f81-c356-11ce-bf01-00aa0055595a");
            //public static readonly Guid MpegVideo   = new Guid("05589f82-c356-11ce-bf01-00aa0055595a");
            //public static readonly Guid MpegStreams = new Guid("05589f83-c356-11ce-bf01-00aa0055595a");
            //public static readonly Guid DvInfo      = new Guid("05589f84-c356-11ce-bf01-00aa0055595a");
        }

        #endregion

        #region DsMarshaler

        abstract internal class DsMarshaler : ICustomMarshaler
        {
            protected string m_cookie;
            protected object m_obj;

            public DsMarshaler(string cookie)
            {
                m_cookie = cookie;
            }

            virtual public IntPtr MarshalManagedToNative(object managedObj)
            {
                m_obj = managedObj;

                int iSize = GetNativeDataSize() + 3;
                IntPtr p = Marshal.AllocCoTaskMem(iSize);

                for (int x = 0; x < iSize / 4; x++)
                {
                    Marshal.WriteInt32(p, x * 4, 0);
                }

                return p;
            }

            virtual public object MarshalNativeToManaged(IntPtr pNativeData)
            {
                return m_obj;
            }

            virtual public void CleanUpNativeData(IntPtr pNativeData)
            {
                if (pNativeData != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pNativeData);
                }
            }

            virtual public void CleanUpManagedData(object managedObj)
            {
                m_obj = null;
            }

            abstract public int GetNativeDataSize();

        }

        #endregion

        #region EMTMarshaler

        internal class EMTMarshaler : DsMarshaler
        {
            public EMTMarshaler(string cookie)
                : base(cookie)
            {
            }

            override public object MarshalNativeToManaged(IntPtr pNativeData)
            {
                AMMediaType[] emt = m_obj as AMMediaType[];

                for (int x = 0; x < emt.Length; x++)
                {
                    IntPtr p = Marshal.ReadIntPtr(pNativeData, x * IntPtr.Size);
                    if (p != IntPtr.Zero)
                    {
                        emt[x] = (AMMediaType)Marshal.PtrToStructure(p, typeof(AMMediaType));
                    }
                    else
                    {
                        emt[x] = null;
                    }
                }

                return null;
            }

            override public int GetNativeDataSize()
            {
                int i = ((Array)m_obj).Length;
                int j = i * IntPtr.Size;
                return j;
            }

            public static ICustomMarshaler GetInstance(string cookie)
            {
                return new EMTMarshaler(cookie);
            }
        }

        #endregion

        #region DsOptInt64

        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public class DsOptInt64
        {
            public DsOptInt64(long Value)
            {
                this.Value = Value;
            }
            public long Value;
        }

        #endregion

        #region VideoInfoHeader

        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public class VideoInfoHeader
        {
            public DsRECT SrcRect;
            public DsRECT TagRect;
            public int BitRate;
            public int BitErrorRate;
            public long AvgTimePerFrame;
            public DsBITMAPINFOHEADER BmiHeader;
        }

        #endregion

        #endregion

        #endregion
    }
}
