// In: src/components/ProtectedRoute.jsx
import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { jwtDecode } from "jwt-decode";

const ProtectedRoute = ({ children, roles }) => {
  const { token } = useAuth();

  if (!token) {
    // If no token, redirect to login
    return <Navigate to="/login" />;
  }

  const decodedToken = jwtDecode(token);
  const userRoles =
    decodedToken[
      "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    ];

  // Ensure userRoles is an array
  const rolesArray = Array.isArray(userRoles) ? userRoles : [userRoles];

  // If the user does not have any of the required roles, redirect
  if (!roles.some((role) => rolesArray.includes(role))) {
    return <Navigate to="/menu" />; // Redirect to a safe page like the menu
  }

  return children;
};

export default ProtectedRoute;
