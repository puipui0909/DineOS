import {Box, Typography, Button} from '@mui/material';
import TableForm from './components/TableForm';
import { useEffect, useState } from 'react';
import TableCard from './components/TableCard';
import StatusSummary from './components/StatusSummary';
import { tableService } from '../../../api/tableService';
import TableHeader from './components/TableHeader';
import { useAuth } from '../../../hooks/useAuth';
import Grid from '@mui/material/Grid';


const Table = () => {
  const [tables, setTables] = useState([]);
  const [open, setOpen] = useState(false);
  const { role } = useAuth();
  const isAdmin = role === 'Admin';
  useEffect(() => {
    fetchTables();
  }, []);

  const fetchTables = async () => {
    try {
      const res = await tableService.getAll();
      setTables(
        res.data.sort((a, b) => {
          const numA = parseInt(a.name.replace('T-', ''));
          const numB = parseInt(b.name.replace('T-', ''));
          return numA - numB;
        })
      );
    } catch (err) {
      console.error(err);
    }};  

  const handleDelete = async (id) => {
    const table = tables.find(t => t.id === id);
    if (table.status === 'Occupied') {
      alert("Cannot delete occupied table");
      return;
    }
    try {
      await tableService.delete(id);
      fetchTables(); // reload
    } catch (err) {
      console.error(err);
    }
  };

  const handleReserve = async (tableId) => {
    console.log("RESERVE API CALL", tableId);
    try {
      await tableService.updateStatus(tableId, "Reserved");
      await fetchTables();
    } catch (err) {
      console.error(err);
    }
  };
  const handleCancelReserve = async (tableId) => {
    try {
      await tableService.updateStatus(tableId, "Available");
      await fetchTables();
    } catch (err) {
      console.error(err);
    }
  };

  const handleToggleOutOfService = async (tableId, isChecked) => {
    try {
      const status = isChecked ? "OutOfService" : "Available";

      await tableService.updateStatus(tableId, status);
      await fetchTables();
    } catch (err) {
      console.error(err);
    }
  };
  return (
    <Box>
      <TableHeader
        isAdmin={isAdmin}
        onAddClick={() => setOpen(true)}
      />
      <TableForm open={open} onClose={() => setOpen(false)} onCreated={fetchTables}/>
      <StatusSummary tables={tables} />

      <Grid container spacing={3}>
        {tables.map(t => (
          <Grid size={{ xs: 12, sm: 6, md: 4, lg: 3 }} key={t.id}>
            <TableCard table={t} 
              onDelete={handleDelete} 
              onReserve={handleReserve}
              onCancelReserve={handleCancelReserve}
              onToggleOutOfService={handleToggleOutOfService}
             />
          </Grid>
        ))}
      </Grid>
    </Box>
  );
};

export default Table;