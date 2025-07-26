// 1. Import the extendTheme function
import { extendTheme } from "@chakra-ui/react";
import * as ReactDOM from "react-dom/client";
import React from "react";
import App from "./App";
import { ChakraProvider } from "@chakra-ui/react";
import theme from "./theme/theme";
import { BrowserRouter } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import { CartProvider } from "./context/CartContext";

// 3. Pass the `theme` prop to the `ChakraProvider`

const rootElement = document.getElementById("root");
ReactDOM.createRoot(rootElement).render(
  <React.StrictMode>
    <ChakraProvider theme={theme}>
      <BrowserRouter>
        <AuthProvider>
          <CartProvider>
            <App />
          </CartProvider>
        </AuthProvider>
      </BrowserRouter>
    </ChakraProvider>
  </React.StrictMode>
);
