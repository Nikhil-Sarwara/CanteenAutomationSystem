import { useState, useEffect } from "react";
import {
  Box,
  Button,
  Container,
  FormControl,
  FormLabel,
  Heading,
  Input,
  VStack,
  useToast,
  SimpleGrid,
  Text,
} from "@chakra-ui/react";
import apiClient from "../apiClient";

const MenuManagement = () => {
  const [menuItems, setMenuItems] = useState([]);
  const [name, setName] = useState("");
  const [price, setPrice] = useState("");
  const [description, setDescription] = useState("");
  const toast = useToast();

  const fetchMenu = async () => {
    const response = await apiClient.get("/api/menu");
    setMenuItems(response.data);
  };

  useEffect(() => {
    fetchMenu();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await apiClient.post("/api/menu", {
        name,
        price: parseFloat(price),
        description,
      });
      toast({
        title: "Menu item added!",
        status: "success",
        duration: 3000,
        isClosable: true,
      });
      fetchMenu(); // Refresh the list
      // Clear form
      setName("");
      setPrice("");
      setDescription("");
    } catch (error) {
      toast({
        title: "Error adding item.",
        status: "error",
        duration: 3000,
        isClosable: true,
      });
    }
  };

  return (
    <Container maxW="container.lg" py={10}>
      <Heading mb={6}>Menu Management</Heading>
      <SimpleGrid columns={{ base: 1, md: 2 }} spacing={10}>
        <Box
          as="form"
          onSubmit={handleSubmit}
          p={6}
          borderWidth={1}
          borderRadius="lg"
        >
          <VStack spacing={4}>
            <Heading size="md">Add New Item</Heading>
            <FormControl isRequired>
              <FormLabel>Name</FormLabel>
              <Input value={name} onChange={(e) => setName(e.target.value)} />
            </FormControl>
            <FormControl isRequired>
              <FormLabel>Price</FormLabel>
              <Input
                type="number"
                value={price}
                onChange={(e) => setPrice(e.target.value)}
              />
            </FormControl>
            <FormControl isRequired>
              <FormLabel>Description</FormLabel>
              <Input
                value={description}
                onChange={(e) => setDescription(e.target.value)}
              />
            </FormControl>
            <Button width="full" type="submit" colorScheme="brand">
              Add Item
            </Button>
          </VStack>
        </Box>
        <Box>
          <Heading size="md" mb={4}>
            Current Menu
          </Heading>
          <VStack align="stretch" spacing={2}>
            {menuItems.map((item) => (
              <Text key={item.id}>
                - {item.name} (${item.price})
              </Text>
            ))}
          </VStack>
        </Box>
      </SimpleGrid>
    </Container>
  );
};

export default MenuManagement;
