// In: src/pages/AdminReports.jsx
import { useEffect, useState } from "react";
import {
  Box,
  Container,
  Heading,
  Stat,
  StatLabel,
  StatNumber,
  StatHelpText,
  SimpleGrid,
  Spinner,
  Alert,
  AlertIcon,
} from "@chakra-ui/react";
import apiClient from "../apiClient";

const AdminReports = () => {
  const [report, setReport] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchReport = async () => {
      try {
        // We will create this backend endpoint in the next step
        const response = await apiClient.get("/api/reports/dailysummary");
        setReport(response.data);
      } catch (err) {
        setError("Failed to fetch reports. You may not have access.");
      } finally {
        setLoading(false);
      }
    };
    fetchReport();
  }, []);

  if (loading) return <Spinner size="xl" />;
  if (error)
    return (
      <Alert status="error">
        <AlertIcon />
        {error}
      </Alert>
    );

  return (
    <Container maxW="container.lg" py={10}>
      <Heading mb={6}>Admin Dashboard: Daily Sales</Heading>
      <SimpleGrid columns={{ base: 1, md: 2 }} spacing={6}>
        <Stat p={4} borderWidth={1} borderRadius="lg">
          <StatLabel>Total Sales Today</StatLabel>
          <StatNumber>${report?.totalSales.toFixed(2)}</StatNumber>
          <StatHelpText>{new Date().toLocaleDateString()}</StatHelpText>
        </Stat>
        <Stat p={4} borderWidth={1} borderRadius="lg">
          <StatLabel>Total Orders Today</StatLabel>
          <StatNumber>{report?.orderCount}</StatNumber>
          <StatHelpText>All completed and pending orders</StatHelpText>
        </Stat>
      </SimpleGrid>
    </Container>
  );
};

export default AdminReports;
