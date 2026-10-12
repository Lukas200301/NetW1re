namespace NetW1reAvalonia.Core.Theme
{
	// Flat, modern dark design tokens.
	// Property names are preserved for existing XAML bindings; values were
	// refreshed to a flat palette (no gradients) with a single refined blue accent.
	public class AppTheme
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
		}

		// Window sizing
		public string WindowWidth => "1440";
		public string WindowMinWidth => "1180";
		public string WindowHeight => "920";
		public string WindowMinHeight => "760";

		// Surfaces
		public string WindowBackground => "#0B0E14";
		public string SurfaceColor => "#12161F";
		public string SurfaceElevated => "#1A1F2B";
		public string SurfaceHover => "#222838";

		// Navigation / chrome
		public string NavBorderColor => "#232A38";
		public string NavBackground => "#10141D";
		public string NavElementBackground => "#1A1F2B";
		public string NavElementBorder => "#232A38";
		public string NavElementText => "#E6EDF3";
		public string TitleBarBackColor => "#10141D";
		public string TitleBarButtonBackColor => "Transparent";
		public string TitleBarButtonFrontColor => "#9FB0C3";
		public string TitleBarButtonsHoverColor => "#222838";

		// Accent (refined single blue)
		public string AccentColor => "#2F88EB";
		public string AccentColorLight => "#4D9BF0";
		public string AccentColorDark => "#1F6FD0";
		public string AccentColorVeryLight => "#8FC2F7";
		public string AccentSubtle => "#15233A";

		// Secondary surfaces (legacy names kept)
		public string SecondaryColor => "#1A1F2B";
		public string SecondaryColorLight => "#222838";
		public string SecondaryColorDark => "#12161F";

		// Borders
		public string BorderColor => "#232A38";
		public string BorderColorStrong => "#30394A";

		// Typography
		public string PageTitleColor => "#F2F6FB";
		public string PageTitleSize => "24";
		public string TextPrimary => "#E6EDF3";
		public string TextSecondary => "#9FB0C3";
		public string TextMuted => "#6B7A8D";

		// Lists
		public string ListRowSelectedBackColor => "#15233A";
		public string ListColumnHeaderBackColor => "#161B26";

		// Spacing / controls
		public string PagePadding => "24";
		public string DialogButtonWidth => "80";
		public string DialogButtonHeight => "40";

		// Status
		public string SuccessColor => "#3FB950";
		public string WarningColor => "#D29922";
		public string ErrorColor => "#F85149";
		public string InfoColor => "#2F88EB";
	}
}
