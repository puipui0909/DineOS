import { Card, CardContent, Typography } from '@mui/material';
import Grid from "@mui/material/Grid";

export default function MenuGrid({ items, onSelect }) {
  return (
    <Grid container spacing={2}>
      {items.map((item) => (
        <Grid size={6} key={item.id}>
          <Card onClick={() => onSelect(item)} sx={{ cursor: 'pointer' }}>
            <CardContent>
              <Typography fontWeight="bold">{item.name}</Typography>
              <Typography color="text.secondary">
                {item.price.toLocaleString()}đ
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      ))}
    </Grid>
  );
}