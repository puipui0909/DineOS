import { useEffect, useState } from 'react';
import { Box, Grid, Card, CardContent, Typography } from '@mui/material';
import { dashboardService } from '../../../api/dashboardService';
import RevenueChart from './RevenueChart';

export default function DashboardPage() {
  const [summary, setSummary] = useState(null);
  const [revenue, setRevenue] = useState([]);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    const summaryData = await dashboardService.getSummary();
    const revenueData = await dashboardService.getRevenueMonthly(2026);

    setSummary(summaryData);
    setRevenue(revenueData);
  };

  const chartData = revenue.map(r => ({
    month: `Thg ${r.month}`,
    total: r.total
  }));

  const total = revenue.reduce((sum, r) => sum + r.total, 0);

  return (
    <Box p={3}>
      {/* KPI */}
      <Grid container spacing={2} mb={3}>
        <Grid item xs={4}>
          <Card>
            <CardContent>
              <Typography>Total Order Today</Typography>
              <Typography variant="h5">{summary?.totalOrders}</Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={4}>
          <Card>
            <CardContent>
              <Typography>Active Table</Typography>
              <Typography variant="h5">
                {summary?.activeTables}/{summary?.totalTables}
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={4}>
          <Card>
            <CardContent>
              <Typography>Revenue Today</Typography>
              <Typography variant="h5">
                {summary?.revenueToday?.toLocaleString()}đ
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Chart */}
      <Card>
        <CardContent>
          <Typography variant="h6">
            Doanh thu năm (VND)
          </Typography>

          <Typography fontWeight="bold" mb={2}>
            {total.toLocaleString()} đ
          </Typography>

          <RevenueChart data={chartData} />
        </CardContent>
      </Card>
    </Box>
  );
}