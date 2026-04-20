import { Box, Typography } from '@mui/material';

const StatusSummary = ({ tables }) => {
  const count = (status) =>
    tables.filter(t => t.status === status).length;

  const items = [
    { label: 'Available', status: 'Available' },
    { label: 'Occupied', status: 'Occupied' },
    { label: 'Reserved', status: 'Reserved' },
    { label: 'Out of service', status: 'OutOfService' },
  ];

  return (
    <Box display="flex" gap={2} mb={3}>
      {items.map(i => (
        <Box
          key={i.status}
          sx={{
            bgcolor: '#f0f0f0',
            px: 3,
            py: 2,
            borderRadius: 2,
            minWidth: 120
          }}
        >
          <Typography variant="body2">{i.label}</Typography>
          <Typography variant="h6">{count(i.status)}</Typography>
        </Box>
      ))}
    </Box>
  );
};

export default StatusSummary;