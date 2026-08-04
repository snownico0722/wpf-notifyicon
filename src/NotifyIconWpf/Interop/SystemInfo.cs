// hardcodet.net NotifyIcon for WPF
// Copyright (c) 2009 - 2022 Philipp Sumi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Contact and Information: http://www.hardcodet.net

using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Hardcodet.Wpf.TaskbarNotification.Interop
{
    /// <summary>
    /// This class is a helper for system information, currently to get the DPI factors
    /// </summary>
    public static class SystemInfo
    {
        /// <summary>
        /// Make sure the initial value is calculated at the first access
        /// </summary>
        static SystemInfo()
        {
            UpdateDpiFactors();
        }

        /// <summary>
        /// This calculates the current DPI values and sets this into the DpiFactorX/DpiFactorY values
        /// </summary>
        internal static void UpdateDpiFactors()
        {
            using (var source = new HwndSource(new HwndSourceParameters()))
            {
                if (source.CompositionTarget?.TransformToDevice != null)
                {
                    DpiFactorX = source.CompositionTarget.TransformToDevice.M11;
                    DpiFactorY = source.CompositionTarget.TransformToDevice.M22;
                    return;
                }
            }

            DpiFactorX = DpiFactorY = 1;
        }

        /// <summary>
        /// Returns the DPI X Factor
        /// </summary>
        public static double DpiFactorX { get; private set; } = 1;

        /// <summary>
        /// Returns the DPI Y Factor
        /// </summary>
        public static double DpiFactorY { get; private set; } = 1;

        /// <summary>
        /// Converts the physical shell anchor into the logical screen coordinate space
        /// used by WPF popup placement.
        /// </summary>
        /// <param name="point">Physical screen coordinate received from the shell.</param>
        /// <returns>Logical screen coordinate for WPF.</returns>
        [Pure]
        public static Point ScaleWithDpi(this Point point)
        {
            try
            {
                var logicalPoint = point;
                var window = WindowFromPhysicalPoint(point);
                if (window != IntPtr.Zero &&
                    PhysicalToLogicalPointForPerMonitorDPI(window, ref logicalPoint))
                {
                    return logicalPoint;
                }
            }
            catch (EntryPointNotFoundException)
            {
                // Keep compatibility with Windows versions that do not expose the
                // per-monitor conversion APIs.
            }
            catch (DllNotFoundException)
            {
                // Keep compatibility with non-Windows design-time environments.
            }

            return new Point
            {
                X = (int)(point.X / DpiFactorX),
                Y = (int)(point.Y / DpiFactorY)
            };
        }

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPhysicalPoint(Point point);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PhysicalToLogicalPointForPerMonitorDPI(
            IntPtr window,
            ref Point point);

        #region SmallIconSize

        private const int CXSMICON = 49;
        private const int CYSMICON = 50;

        /// <summary>
        /// Gets a value indicating the recommended size, in pixels, of a small icon
        /// </summary>
        public static Size SmallIconSize =>
            new()
            {
                Height = WinApi.GetSystemMetrics(CYSMICON),
                Width = WinApi.GetSystemMetrics(CXSMICON)
            };

        #endregion
    }
}