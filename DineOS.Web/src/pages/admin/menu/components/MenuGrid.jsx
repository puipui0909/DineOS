import Grid from "@mui/material/Grid";
import MenuCard from './MenuCard';

const MenuGrid = ({ menuItems, onEdit, onDelete, onToggle, role }) => {
  return (
    <Grid container spacing={3}>
      {menuItems.map((item) => (
        <Grid
          key={item.id}
          size={{ xs: 12, sm: 6, md: 4, lg: 3 }}
        >
          <MenuCard item={item} onEdit={onEdit} onDelete={onDelete} onToggle={onToggle} role={role} />
        </Grid>
      ))}
    </Grid>
  );
};

export default MenuGrid;