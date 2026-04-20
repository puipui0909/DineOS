// hooks/useMenuData.js
import { useEffect, useState } from 'react';
import { menuService } from '../api/menuService';
import { categoryService } from '../api/categoryService';

export const useMenuData = (tableId) => {
  const [categories, setCategories] = useState([]);
  const [menuItems, setMenuItems] = useState([]);
  const [loading, setLoading] = useState(false);

  const fetchCategories = async () => {
    const data = await categoryService.getAllPublic(tableId);
    setCategories([{ id: 'All', name: 'All' }, ...data]);
  };

  const fetchMenu = async () => {
    if (!tableId) return;
    try {
        setLoading(true);
        const data = await menuService.getByTable(tableId);
        setMenuItems(data);
    } catch (err) {
        console.error("Fetch menu error:", err);
    } finally {
        setLoading(false);
    }
  };

  useEffect(() => {
    fetchCategories();
    fetchMenu();
  }, [tableId]);

  return {
    categories,
    menuItems,
    loading,
    fetchMenu
  };
};