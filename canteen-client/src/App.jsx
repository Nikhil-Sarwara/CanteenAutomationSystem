import { Box } from "@chakra-ui/react";
import { Routes, Route } from "react-router-dom";
import { Heading } from "@chakra-ui/react";
import Navbar from "./components/Navbar";
import Login from "./pages/Login";
import Register from "./pages/Register";

function App() {
  return (
    <Box>
      <Navbar />
      <main>
        <Routes>
          <Route
            path="/"
            element={
              <Heading textAlign="center" mt={10}>
                Welcome Home!
              </Heading>
            }
          />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
        </Routes>
      </main>
    </Box>
  );
}

export default App;
