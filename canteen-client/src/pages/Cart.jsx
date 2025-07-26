import {
  Box,
  Button,
  Container,
  Divider,
  Heading,
  HStack,
  Text,
  VStack,
  useToast,
  Spinner,
} from "@chakra-ui/react";
import { useCart } from "../context/CartContext";
import { useNavigate } from "react-router-dom";
import apiClient from "../apiClient";

const Cart = () => {
  const { cartItems } = useCart();
  const navigate = useNavigate();
  const toast = useToast();

  // Calculate total price
  const totalPrice = cartItems
    .reduce((total, item) => total + item.price * item.quantity, 0)
    .toFixed(2);

  const handlePlaceOrder = async () => {
    // Format the cart items for the backend API
    const orderRequest = {
      items: cartItems.map((item) => ({
        menuItemId: item.id,
        quantity: item.quantity,
      })),
    };

    try {
      await apiClient.post("/api/order", orderRequest);
      toast({
        title: "Order placed!",
        description: "Your order has been sent to the kitchen.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      // Here you would typically clear the cart and redirect
      // For now, we'll just navigate to a success page
      navigate("/order-confirmation");
    } catch (error) {
      toast({
        title: "Order failed.",
        description: "There was a problem submitting your order.",
        status: "error",
        duration: 5000,
        isClosable: true,
      });
    }
  };

  if (cartItems.length === 0) {
    return (
      <Container centerContent>
        <Heading mt={10}>Your cart is empty.</Heading>
      </Container>
    );
  }

  return (
    <Container maxW="container.md" py={10}>
      <VStack spacing={6} align="stretch">
        <Heading>Your Cart</Heading>
        <Box p={4} borderWidth={1} borderRadius="lg">
          <VStack spacing={4} align="stretch" divider={<Divider />}>
            {cartItems.map((item) => (
              <HStack key={item.id} justifyContent="space-between">
                <Box>
                  <Text fontWeight="bold">{item.name}</Text>
                  <Text fontSize="sm">Qty: {item.quantity}</Text>
                </Box>
                <Text fontWeight="semibold">
                  ${(item.price * item.quantity).toFixed(2)}
                </Text>
              </HStack>
            ))}
          </VStack>
        </Box>
        <HStack justifyContent="space-between">
          <Text fontSize="xl" fontWeight="bold">
            Total:
          </Text>
          <Text fontSize="xl" fontWeight="bold">
            ${totalPrice}
          </Text>
        </HStack>
        <Button size="lg" colorScheme="brand" onClick={handlePlaceOrder}>
          Place Order
        </Button>
      </VStack>
    </Container>
  );
};

export default Cart;
