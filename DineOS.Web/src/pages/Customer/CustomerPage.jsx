import { useEffect, useState } from 'react';
import { Box, CircularProgress, TextField, Button, Grid } from '@mui/material';
import { useSearchParams } from 'react-router-dom';

import { orderService } from '../../api/orderService';
import { menuService } from '../../api/menuService';
import { useMenuData } from '../../hooks/useMenuData';
import { useMenuFilter } from '../../hooks/useMenuFilter';

import MenuCard from '../admin/menu/components/MenuCard';
import MenuGrid from './components/MenuGrid';
import ItemDetailModal from './components/ItemDetailModal';
import CartButton from './components/CartButton';
import OrderSummary from './components/OrderSummary';

export default function CustomerPage() {
  const [searchParams] = useSearchParams();
  const tableId = searchParams.get('tableId');

  const { menuItems, categories } = useMenuData(tableId);
  const [order, setOrder] = useState(null);
  const [searchText, setSearchText] = useState('');
  const [selectedCategory, setSelectedCategory] = useState('All');
  const [selectedItem, setSelectedItem] = useState(null);
  const [openItemModal, setOpenItemModal] = useState(false);
  const [openCart, setOpenCart] = useState(false);
  const [cartItems, setCartItems] = useState([]);

  const [loading, setLoading] = useState(false);

  const filteredMenu = useMenuFilter(
    menuItems,
    searchText,
    selectedCategory
  );

  const fetchOrder = async () => {
    try {
      const data = await orderService.customer.getByTable(tableId);
      console.log('ORDER AFTER ADD:', data);
      setOrder(data);
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => {
    if (!tableId) return;
    fetchOrder();
  }, [tableId]);

  const handleSelectItem = (item) => {
    setSelectedItem(item);
    setOpenItemModal(true);
  };

  const handleAddItem = (menuItemId, quantity) => {
    const item = menuItems.find(i => i.id === menuItemId);

    setCartItems(prev => {
      const existing = prev.find(i => i.menuItemId === menuItemId);

      if (existing) {
        return prev.map(i =>
          i.menuItemId === menuItemId
            ? { ...i, quantity: i.quantity + quantity }
            : i
        );
      }

      return [
        ...prev,
        {
          menuItemId,
          name: item.name,
          price: item.price,
          quantity
        }
      ];
    });

    setOpenItemModal(false);
  };

  const handleSendToKitchen = async () => {
    try {
      setLoading(true);

      const payload = {
        tableId,
        items: cartItems.map(i => ({
          menuItemId: i.menuItemId,
          quantity: i.quantity
        }))
      };

      await orderService.customer.createOrder(payload);

      setCartItems([]);
      await fetchOrder();

    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box p={2}>
      <TextField
        fullWidth
        placeholder="Tìm món..."
        value={searchText}
        onChange={(e) => setSearchText(e.target.value)}
        sx={{ mb: 2 }}
      />

      <Box display="flex" gap={1} mb={2} flexWrap="wrap">
        {categories.map(c => (
          <Button
            key={c.id}
            variant={selectedCategory === c.id ? 'contained' : 'outlined'}
            onClick={() => setSelectedCategory(c.id)}
          >
            {c.name}
          </Button>
        ))}
      </Box>
      {/* Thay thế đoạn hiển thị MenuCard cũ bằng đoạn này */}
      <Grid container spacing={2}> 
        {filteredMenu?.map((item) => (
          <Grid size={{ xs: 6, sm: 4, md: 3 }} key={item.id}> 
            <MenuCard
              item={item}
              mode="order"
              onClick={handleSelectItem}
            />
          </Grid>
        ))}
      </Grid>

      <CartButton
        count={cartItems.length}
        onClick={() => setOpenCart(true)}
      />

      <ItemDetailModal
        open={openItemModal}
        item={selectedItem}
        onClose={() => setOpenItemModal(false)}
        onConfirm={handleAddItem}
      />

      <OrderSummary
        open={openCart}
        order={{ orderItems: cartItems }}
        onClose={() => setOpenCart(false)}
        onSend={handleSendToKitchen}
      />
    </Box>
  );
}