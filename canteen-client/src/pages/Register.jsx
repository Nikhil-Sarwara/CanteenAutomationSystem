import { useState } from "react";
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
} from "@chakra-ui/react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

const API_URL = "http://localhost:5154"; // Your backend URL

const Register = () => {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const toast = useToast();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await axios.post(`${API_URL}/api/auth/register`, {
        username,
        email,
        password,
      });
      toast({
        title: "Account created.",
        description: "We've created your account for you.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      navigate("/login"); // Redirect to login page on success
    } catch (error) {
      toast({
        title: "An error occurred.",
        description:
          error.response?.data?.errors?.[0]?.description ||
          "Unable to create account.",
        status: "error",
        duration: 5000,
        isClosable: true,
      });
    }
  };

  return (
    <Container centerContent>
      <Box
        as="form"
        onSubmit={handleSubmit}
        w="100%"
        maxW="md"
        p={8}
        mt={10}
        borderWidth={1}
        borderRadius="lg"
        boxShadow="lg"
      >
        <VStack spacing={4}>
          <Heading>Create Your Account</Heading>
          <FormControl isRequired>
            <FormLabel>Username</FormLabel>
            <Input
              type="text"
              placeholder="Your username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
          </FormControl>
          <FormControl isRequired>
            <FormLabel>Email</FormLabel>
            <Input
              type="email"
              placeholder="Your email address"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </FormControl>
          <FormControl isRequired>
            <FormLabel>Password</FormLabel>
            <Input
              type="password"
              placeholder="Choose a password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </FormControl>
          <Button width="full" mt={4} type="submit">
            Register
          </Button>
        </VStack>
      </Box>
    </Container>
  );
};

export default Register;
