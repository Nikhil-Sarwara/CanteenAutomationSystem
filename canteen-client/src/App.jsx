import { Box } from "@chakra-ui/react";
import { Routes, Route } from "react-router-dom";
import { Heading } from "@chakra-ui/react";
import Navbar from "./components/Navbar";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Cart from "./pages/Cart";
import OrderConfirmation from "./pages/OrderConfirmation";
import Menu from "./pages/Menu";
import StaffDashboard from "./pages/StaffDashboard";
import AdminReports from "./pages/AdminReports";
import ProtectedRoute from "./components/ProtectedRoute";
import MenuManagement from "./pages/MenuManagement";
import Home from "./pages/Home";

function App() {
  return (
    <Box>
      <Navbar />
      <main>
        <Routes>
          {/* Public Routes */}
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />

          {/* User Routes */}
          <Route
            path="/menu"
            element={
              <ProtectedRoute roles={["User", "Admin", "Staff"]}>
                <Menu />
              </ProtectedRoute>
            }
          />
          <Route
            path="/cart"
            element={
              <ProtectedRoute roles={["User", "Admin"]}>
                <Cart />
              </ProtectedRoute>
            }
          />
          <Route
            path="/order-confirmation"
            element={
              <ProtectedRoute roles={["User", "Admin"]}>
                <OrderConfirmation />
              </ProtectedRoute>
            }
          />

          {/* Staff & Admin Routes */}
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute roles={["Admin", "Staff"]}>
                <StaffDashboard />
              </ProtectedRoute>
            }
          />

          {/* Admin Only Routes */}
          <Route
            path="/admin/menu"
            element={
              <ProtectedRoute roles={["Admin"]}>
                <MenuManagement />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admin/reports"
            element={
              <ProtectedRoute roles={["Admin"]}>
                <AdminReports />
              </ProtectedRoute>
            }
          />
        </Routes>
      </main>
    </Box>
  );
}

export default App;
