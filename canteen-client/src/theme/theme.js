import { extendTheme } from "@chakra-ui/react";
import { mode } from "@chakra-ui/theme-tools";

const theme = extendTheme({
  // -----------------------------------------------------------------
  // 1. Color Palette
  // -----------------------------------------------------------------
  colors: {
    // Primary brand color: A warm, appetizing orange.
    brand: {
      50: "#fff8e1",
      100: "#ffecb3",
      200: "#ffd54f",
      300: "#ffc107",
      400: "#ffb300",
      500: "#ffa000", // Main brand color
      600: "#ff8f00",
      700: "#ff6f00",
      800: "#f57f17",
      900: "#e65100",
    },
    // Accent color: An earthy, fresh green.
    accent: {
      50: "#f1f8e9",
      100: "#dcedc8",
      200: "#c5e1a5",
      300: "#aed581",
      400: "#9ccc65",
      500: "#8bc34a", // Main accent color
      600: "#7cb342",
      700: "#689f38",
      800: "#558b2f",
      900: "#33691e",
    },
    // Using slightly warm grays to complement the brand colors
    gray: {
      50: "#f9f9f9",
      100: "#ededed",
      200: "#d3d3d3",
      300: "#b9b9b9",
      400: "#a0a0a0",
      500: "#868686",
      600: "#6d6d6d",
      700: "#535353",
      800: "#3a3a3a",
      900: "#202020",
    },
  },

  // -----------------------------------------------------------------
  // 2. Typography
  // -----------------------------------------------------------------
  fonts: {
    heading: `'Poppins', sans-serif`,
    body: `'Inter', sans-serif`,
  },

  // -----------------------------------------------------------------
  // 3. Global Styles
  // -----------------------------------------------------------------
  styles: {
    global: (props) => ({
      body: {
        fontFamily: "body",
        color: mode("gray.800", "whiteAlpha.900")(props),
        bg: mode("gray.50", "gray.900")(props), // Soft gray background
        lineHeight: "base",
      },
      "*::placeholder": {
        color: mode("gray.400", "whiteAlpha.400")(props),
      },
      "*, *::before, &::after": {
        borderColor: mode("gray.200", "whiteAlpha.300")(props),
        wordWrap: "break-word",
      },
    }),
  },

  // -----------------------------------------------------------------
  // 4. Component Overrides
  // -----------------------------------------------------------------
  components: {
    // ---- Button ----
    Button: {
      baseStyle: {
        fontWeight: "semibold", // Buttons are important actions
        borderRadius: "md", // Softer corners
      },
      variants: {
        solid: {
          bg: "brand.500",
          color: "white",
          _hover: {
            bg: "brand.600",
          },
          _active: {
            bg: "brand.700",
          },
        },
        outline: {
          borderColor: "brand.500",
          color: "brand.500",
          _hover: {
            bg: "brand.50",
          },
        },
      },
    },

    // ---- Heading ----
    Heading: {
      baseStyle: {
        fontFamily: "heading",
        fontWeight: "bold",
        color: "gray.800",
      },
    },

    // ---- Card (Custom Component for Menu Items) ----
    Card: {
      baseStyle: {
        container: {
          bg: "white",
          borderRadius: "lg",
          boxShadow: "md",
          overflow: "hidden",
        },
      },
      variants: {
        outline: {
          container: {
            boxShadow: "none",
            border: "1px solid",
            borderColor: "gray.200",
          },
        },
      },
    },

    // ---- Input Fields ----
    Input: {
      variants: {
        outline: {
          field: {
            bg: "white",
            borderColor: "gray.300",
            _hover: {
              borderColor: "gray.400",
            },
            _focus: {
              borderColor: "brand.500",
              boxShadow: `0 0 0 1px var(--chakra-colors-brand-500)`,
            },
          },
        },
      },
      defaultProps: {
        variant: "outline",
      },
    },

    // ---- Tags (for dietary info, etc.) ----
    Tag: {
      baseStyle: {
        container: {
          borderRadius: "full", // Fully rounded for a modern "pill" look
        },
      },
      variants: {
        // Custom variant for 'Vegan' or 'Healthy'
        success: {
          container: {
            bg: "accent.100",
            color: "accent.800",
          },
        },
        // Custom variant for 'Spicy' or 'Allergen'
        danger: {
          container: {
            bg: "red.100",
            color: "red.800",
          },
        },
      },
    },
  },

  // -----------------------------------------------------------------
  // 5. Config
  // -----------------------------------------------------------------
  config: {
    initialColorMode: "light", // Force light mode as it's best for food apps
    useSystemColorMode: false,
  },
});

export default theme;
