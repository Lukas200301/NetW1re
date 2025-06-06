namespace NetW1reAvalonia.Core.Theme
{	public class AppTheme
	{
		private static AppTheme? _instance;

		public AppTheme() { }

		public static AppTheme Instance
		{
			get
			{
				_instance ??= new AppTheme();
				return _instance;
			}
		}		public string WindowWidth => "1400";
		public string WindowMinWidth => "1200";
		public string WindowHeight => "900";
		public string WindowMinHeight => "800";
		public string NavBorderColor => "#2A3441";
		public string NavBackground => "#1A2332";
		public string NavElementBackground => "#2A3441";
		public string NavElementBorder => "#1A2332";
		public string NavElementText => "#E8F4FD";
		public string WindowBackground => "#0F1419";
		public string TitleBarBackColor => "#0F1419";
		public string TitleBarButtonBackColor => "#0F1419";
		public string TitleBarButtonFrontColor => "#E8F4FD";
		public string TitleBarButtonsHoverColor => "#136999";
		public string PageTitleColor => "#FFFFFF";
		public string AccentColor => "#136999";
		public string AccentColorLight => "#4A9BD1";
		public string AccentColorDark => "#0D4F73";
		public string AccentColorVeryLight => "#7DB8E8";
		public string SecondaryColor => "#2A3441";
		public string SecondaryColorLight => "#3D4B5C";
		public string SecondaryColorDark => "#1A2332";
		public string PageTitleSize => "26";
		public string ListRowSelectedBackColor => "#136999";
		public string ListColumnHeaderBackColor => "#2A3441";
		public string PagePadding => "20";
		public string DialogButtonWidth => "70";
		public string DialogButtonHeight => "40";
		public string SuccessColor => "#28A745";
		public string WarningColor => "#FFC107";
		public string ErrorColor => "#DC3545";
		public string InfoColor => "#136999";
		public string TextPrimary => "#E8F4FD";
		public string TextSecondary => "#B8CDE8";
		public string TextMuted => "#7A8BA3";
	}
}
