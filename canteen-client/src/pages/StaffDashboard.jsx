// In: src/pages/StaffDashboard.jsx
import { useEffect, useState } from "react";
import {
  Box,
  Container,
  Heading,
  SimpleGrid,
  Text,
  Card,
  CardBody,
  VStack,
  HStack,
  Divider,
  Badge,
} from "@chakra-ui/react";
import { HubConnectionBuilder } from "@microsoft/signalr";

const API_URL = "http://localhost:8080"; // Your backend URL

const StaffDashboard = () => {
  const [orders, setOrders] = useState([]);

  useEffect(() => {
    // Establish a connection to the SignalR hub
    const connection = new HubConnectionBuilder()
      .withUrl(`${API_URL}/orderHub`)
      .withAutomaticReconnect()
      .build();

    // Set up a listener for the "ReceiveOrder" event from the backend
    connection.on("ReceiveOrder", (newOrder) => {
      // Add the new order to the top of the list
      setOrders((prevOrders) => [newOrder, ...prevOrders]);
    });

    // Start the connection
    connection
      .start()
      .catch((err) => console.error("SignalR Connection Error: ", err));

    // Clean up the connection when the component unmounts
    return () => {
      connection.stop();
    };
  }, []); // The empty array ensures this effect runs only once

  return (
    <Container maxW="container.xl" py={10}>
      <Heading mb={6}>Live Orders</Heading>
      <SimpleGrid columns={{ base: 1, md: 2, lg: 3 }} spacing={6}>
        {orders.map((order) => (
          <Card
            key={order.id}
            bg="brand.50"
            borderWidth="2px"
            borderColor="brand.200"
          >
            <CardBody>
              <VStack align="stretch" spacing={3}>
                <HStack justifyContent="space-between">
                  <Heading size="md">Order #{order.id}</Heading>
                  <Badge colorScheme="green">{order.status}</Badge>
                </HStack>
                <Divider />
                {order.orderItems.map((item) => (
                  <HStack key={item.menuItemId} justifyContent="space-between">
                    <Text>
                      {item.quantity} x {item.menuItemName}
                    </Text>
                    <Text>${(item.price * item.quantity).toFixed(2)}</Text>
                  </HStack>
                ))}
                <Divider />
                <HStack justifyContent="space-between">
                  <Text fontWeight="bold">Total:</Text>
                  <Text fontWeight="bold">${order.totalAmount.toFixed(2)}</Text>
                </HStack>
              </VStack>
            </CardBody>
          </Card>
        ))}
      </SimpleGrid>
    </Container>
  );
};

export default StaffDashboard;
