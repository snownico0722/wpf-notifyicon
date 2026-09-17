// hardcodet.net NotifyIcon for WPF
// Copyright (c) 2009 - 2022 Philipp Sumi. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Hardcodet.Wpf.TaskbarNotification
{
    public partial class TaskbarIcon
    {
        /// <summary>
        /// Displays the configured <see cref="ContextMenu"/> at the current mouse position.
        /// This uses the same popup lifecycle, DPI handling and activation path as a tray-originated
        /// context-menu request, and remains available while the notification-area icon is hidden.
        /// </summary>
        public void ShowContextMenuAtCurrentMousePosition()
        {
            ShowContextMenuAtMouse();
        }
    }
}
