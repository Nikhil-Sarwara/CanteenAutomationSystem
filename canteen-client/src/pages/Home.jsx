import {
  Box,
  Button,
  Container,
  Flex,
  Heading,
  Image,
  Link,
  Text,
} from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";

export default function Home() {
  const navigate = useNavigate();

  return (
    <Container maxW="container.xl">
      <Flex
        flexDirection="column"
        justifyContent="center"
        alignItems="center"
        pt={20}
        pb={20}
      >
        <Image src="/logo.png" alt="Canteen Logo" w="200px" h="auto" mb={10} />
        <Heading as="h1" size="4xl" textAlign="center" mb={5}>
          Welcome to Canteen
        </Heading>
        <Text fontSize="lg" textAlign="center" mb={10}>
          Your one-stop shop for ordering food from your favorite canteens.
        </Text>
        <Link to="/menu">
          <Button
            variant="solid"
            colorScheme="brand"
            size="lg"
            onClick={() => navigate("/menu")}
          >
            Get Started
          </Button>
        </Link>
      </Flex>
    </Container>
  );
}
