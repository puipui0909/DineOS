import { useEffect, useState } from 'react';
import { menuService } from '../api/menuService';
import { categoryService } from '../api/categoryService';

export const useMenuPage = () => {
  const [categories, setCategories] = useState([]);
  const [menuItems, setMenuItems] = useState([]);
  const [loading, setLoading] = useState(false);

  const [editingItem, setEditingItem] = useState(null);
  const [deleteId, setDeleteId] = useState(null);

  const fetchCategories = async () => {
    const data = await categoryService.getAll();
    setCategories([{ id: 'All', name: 'All' }, ...data]);
  };

  const fetchMenu = async () => {
    setLoading(true);
    try {
      const data = await menuService.getAll();
      setMenuItems(data);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchMenu();
    fetchCategories();
  }, []);

  const handleEdit = (item) => {
    setEditingItem(item);
  };

  const handleDelete = async (id) => {
    await menuService.delete(id);
    setMenuItems(prev => prev.filter(x => x.id !== id));
  };

  return {
    categories,
    menuItems,
    loading,

    editingItem,
    setEditingItem,

    deleteId,
    setDeleteId,

    fetchMenu,

    handleEdit,
    handleDelete
  };
};