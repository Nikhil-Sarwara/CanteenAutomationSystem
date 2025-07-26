// In: src/pages/Menu.jsx
import { useEffect, useState } from "react";
import {
  Box,
  Container,
  Heading,
  SimpleGrid,
  Text,
  Card,
  CardBody,
  Image,
  Stack,
  Button,
  Spinner,
  Alert,
  AlertIcon,
  Skeleton,
} from "@chakra-ui/react";
import apiClient from "../apiClient"; // Use our new API client

import { useCart } from "../context/CartContext"; // Import useCart hook
import { useToast } from "@chakra-ui/react";

const Menu = () => {
  const [menuItems, setMenuItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const { addToCart } = useCart(); // Get the addToCart function
  const toast = useToast();

  const handleAddToCart = (item) => {
    addToCart(item);
    toast({
      title: `${item.name} added to cart.`,
      status: "success",
      duration: 2000,
      isClosable: true,
    });
  };

  useEffect(() => {
    const fetchMenu = async () => {
      try {
        const response = await apiClient.get("/api/menu");
        setMenuItems(response.data);
      } catch (err) {
        setError("Failed to fetch menu. Please try again later.");
      } finally {
        setLoading(false);
      }
    };

    fetchMenu();
  }, []); // The empty array ensures this runs only once when the component mounts

  if (loading) {
    return (
      <SimpleGrid columns={{ base: 1, md: 4 }} spacing={6}>
        {[...Array(4)].map((_, i) => (
          <Skeleton key={i} height="300px" borderRadius="lg" />
        ))}
      </SimpleGrid>
    );
  }

  if (error) {
    return (
      <Alert status="error">
        <AlertIcon />
        {error}
      </Alert>
    );
  }

  return (
    <Container maxW="container.xl" py={10}>
      <Heading mb={6}>Our Menu</Heading>
      <SimpleGrid columns={{ base: 1, md: 2, lg: 4 }} spacing={6}>
        {menuItems.map((item) => (
          <Card key={item.id}>
            <CardBody>
              <Image
                src={`https://via.placeholder.com/150?text=${item.name}`} // Placeholder image
                alt={item.name}
                borderRadius="lg"
              />
              <Stack mt="6" spacing="3">
                <Heading size="md">{item.name}</Heading>
                <Text>{item.description}</Text>
                <Text color="brand.600" fontSize="2xl">
                  ${item.price}
                </Text>
              </Stack>
            </CardBody>
            <Button
              variant="solid"
              colorScheme="brand"
              m={4}
              onClick={() => handleAddToCart(item)}
            >
              Add to cart
            </Button>
          </Card>
        ))}
      </SimpleGrid>
    </Container>
  );
};

export default Menu;
