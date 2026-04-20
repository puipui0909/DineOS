import { Fab, Badge } from '@mui/material';
import ShoppingCartIcon from '@mui/icons-material/ShoppingCart';

export default function CartButton({ count, onClick }) {
  return (
    <Fab
      color="primary"
      onClick={onClick}
      sx={{
        position: 'fixed',
        bottom: 20,
        right: 20
      }}
    >
      <Badge badgeContent={count} color="error">
        <ShoppingCartIcon />
      </Badge>
    </Fab>
  );
}