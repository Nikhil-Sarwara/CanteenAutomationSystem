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
import { useAuth } from "../context/AuthContext";

const API_URL = "http://localhost:5154";

const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const toast = useToast();
  const navigate = useNavigate();
  const { login } = useAuth(); // Get the login function from context

  const handleSubmit = async (e) => {
    setIsLoading(true); // Start loading
    e.preventDefault();
    try {
      const response = await axios.post(`${API_URL}/api/auth/login`, {
        username,
        password,
      });

      const token = response.data.token;
      login(token); // Save token to global state and local storage

      const profileResponse = await axios.get(`${API_URL}/api/auth/profile`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      const userRoles = profileResponse.data.roles;
      console.log("User roles:", profileResponse);

      let redirectTo = "/"; // Default redirect for 'User'

      if (userRoles) {
        // Ensure userRoles is an array for consistent checking
        const rolesArray = Array.isArray(userRoles) ? userRoles : [userRoles];

        // Prioritize redirection based on roles (e.g., Admin > Staff > User)
        if (rolesArray.includes("Admin")) {
          redirectTo = "/admin-dashboard";
        } else if (rolesArray.includes("Staff")) {
          redirectTo = "/staff-dashboard";
        }
        // If neither Admin nor Staff, it defaults to "/" (for "User")
      }

      navigate(redirectTo); // Redirect to the appropriate page
    } catch (error) {
      console.error("Login error:", error); // Log the actual error for debugging
      toast({
        title: "Login failed.",
        description:
          error.response?.data?.message || "Invalid username or password.", // Use backend message if available
        status: "error",
        duration: 5000,
        isClosable: true,
      });
    } finally {
      setIsLoading(false); // Stop loading
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
        bg="white"
      >
        <VStack spacing={6}>
          <Heading as="h1" size="xl" mb={4} color="teal.500">
            Canteen Login
          </Heading>

          <FormControl isRequired>
            <FormLabel htmlFor="username">Username</FormLabel>
            <Input
              id="username"
              type="text"
              placeholder="Enter your username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              focusBorderColor="teal.400"
            />
          </FormControl>

          <FormControl isRequired>
            <FormLabel htmlFor="password">Password</FormLabel>
            <Input
              id="password"
              type="password"
              placeholder="Enter your password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              focusBorderColor="teal.400"
            />
          </FormControl>

          <Button
            isLoading={isLoading}
            loadingText="Logging In..."
            colorScheme="teal"
            size="lg"
            width="full"
            mt={4}
            type="submit"
          >
            Login
          </Button>
        </VStack>
      </Box>
    </Container>
  );
};

export default Login;
