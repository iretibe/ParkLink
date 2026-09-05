using MudBlazor;

namespace ParkLink.Shared.Theme
{
    public static class ParkLinkTheme
    {
        public static MudTheme Default { get; } = new()
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#2962FF",
                PrimaryContrastText = "#FFFFFF",

                Secondary = "#00ACC1",
                SecondaryContrastText = "#FFFFFF",

                Tertiary = "#7B1FA2",
                TertiaryContrastText = "#FFFFFF",

                Success = "#2E7D32",
                Warning = "#ED6C02",
                Error = "#D32F2F",
                Info = "#0288D1",

                Background = "#F5F7FA",
                Surface = "#FFFFFF",

                DrawerBackground = "#0B2053",
                DrawerText = "#FFFFFF",
                DrawerIcon = "#FFFFFF",

                AppbarBackground = "#FFFFFF",
                AppbarText = "#263238",

                TextPrimary = "#263238",
                TextSecondary = "#607D8B",

                Divider = "#E0E0E0",
                DividerLight = "#EEEEEE",

                TableLines = "#E0E0E0",
                TableStriped = "#FAFAFA",

                ActionDefault = "#607D8B",
                ActionDisabled = "#BDBDBD",

                HoverOpacity = 0.08
            },

            PaletteDark = new PaletteDark
            {
                Primary = "#5C7CFF",
                Secondary = "#26C6DA",
                Tertiary = "#AB69D7",

                Success = "#66BB6A",
                Warning = "#FFA726",
                Error = "#EF5350",
                Info = "#29B6F6",

                Background = "#0F172A",
                Surface = "#1E293B",

                DrawerBackground = "#08183F",
                DrawerText = "#FFFFFF",
                DrawerIcon = "#FFFFFF",

                AppbarBackground = "#1E293B",
                AppbarText = "#FFFFFF",

                TextPrimary = "#FFFFFF",
                TextSecondary = "#B0BEC5",

                Divider = "#334155"
            },

            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontFamily =
                    [
                        "Inter",
                    "Roboto",
                    "Arial",
                    "sans-serif"
                    ]
                },

                H1 = new H1Typography
                {
                    FontSize = "2.25rem",
                    FontWeight = "700"
                },

                H2 = new H2Typography
                {
                    FontSize = "1.875rem",
                    FontWeight = "700"
                },

                H3 = new H3Typography
                {
                    FontSize = "1.5rem",
                    FontWeight = "600"
                },

                H4 = new H4Typography
                {
                    FontSize = "1.25rem",
                    FontWeight = "600"
                },

                H5 = new H5Typography
                {
                    FontSize = "1.125rem",
                    FontWeight = "600"
                },

                H6 = new H6Typography
                {
                    FontSize = "1rem",
                    FontWeight = "600"
                },

                Body1 = new Body1Typography
                {
                    FontSize = "0.95rem"
                },

                Body2 = new Body2Typography
                {
                    FontSize = "0.875rem"
                }
            },

            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "8px",
                DrawerWidthLeft = "260px",
                DrawerWidthRight = "260px",
                AppbarHeight = "64px"
            },

            ZIndex = new ZIndex
            {
                Drawer = 1200,
                AppBar = 1300,
                Dialog = 1400,
                Snackbar = 1500
            }
        };
    }
}
