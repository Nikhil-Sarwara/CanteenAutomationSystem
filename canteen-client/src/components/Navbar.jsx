import { Box, Flex, Heading, Button, Spacer } from "@chakra-ui/react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext"; // Import useAuth hook
import { useCart } from "../context/CartContext";
import { Icon } from "@chakra-ui/react";
import { FaShoppingCart } from "react-icons/fa";
import { Badge } from "@chakra-ui/react";

const Navbar = () => {
  const { isAuthenticated, logout } = useAuth(); // Get auth state and logout function
  const { cartItems } = useCart();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  // Calculate total number of items
  const totalItems = cartItems.reduce(
    (total, item) => total + item.quantity,
    0
  );

  return (
    <Box bg="white" px={4} boxShadow="sm">
      <Flex h={16} alignItems="center" justifyContent="space-between">
        <Box>
          <Heading as={Link} to="/" size="md" color="brand.700">
            CanteenApp
          </Heading>
        </Box>
        <Spacer />
        <Box display="flex" alignItems="center">
          {isAuthenticated ? (
            <>
              <Button
                as={Link}
                to="/menu"
                colorScheme="brand"
                variant="ghost"
                mr={4}
              >
                Menu
              </Button>
              {/* 3. Add Cart Icon with Badge */}
              <Button as={Link} to="/cart" variant="ghost" mr={4}>
                <Icon as={FaShoppingCart} w={6} h={6} color="gray.600" />
                {totalItems > 0 && (
                  <Badge
                    ml="-1"
                    mt="-5"
                    fontSize="0.8em"
                    colorScheme="brand"
                    borderRadius="full"
                    px={2}
                  >
                    {totalItems}
                  </Badge>
                )}
              </Button>
              <Button
                onClick={handleLogout}
                colorScheme="brand"
                variant="solid"
              >
                Logout
              </Button>
            </>
          ) : (
            <>
              <Button
                as={Link}
                to="/login"
                colorScheme="brand"
                variant="ghost"
                mr={4}
              >
                Login
              </Button>
              <Button
                as={Link}
                to="/register"
                colorScheme="brand"
                variant="solid"
              >
                Register
              </Button>
            </>
          )}
        </Box>
      </Flex>
    </Box>
  );
};

export default Navbar;
