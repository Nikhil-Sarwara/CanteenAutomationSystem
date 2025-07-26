import {
  Box,
  Button,
  Container,
  Heading,
  Text,
  VStack,
} from "@chakra-ui/react";
import { Link } from "react-router-dom";

const OrderConfirmation = () => {
  return (
    <Container centerContent py={20}>
      <VStack spacing={4}>
        <Heading>Thank You!</Heading>
        <Text>Your order has been successfully placed.</Text>
        <Button as={Link} to="/menu" colorScheme="brand">
          Order More
        </Button>
      </VStack>
    </Container>
  );
};

export default OrderConfirmation;
