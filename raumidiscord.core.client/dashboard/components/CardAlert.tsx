import AutoAwesomeRoundedIcon from '@mui/icons-material/AutoAwesomeRounded';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Typography from '@mui/material/Typography';

export default function CardAlert() {
  return (
    <Card variant="outlined" sx={{ m: 1.5, flexShrink: 0 }}>
      <CardContent>
        <AutoAwesomeRoundedIcon fontSize="small" />
        <Typography gutterBottom sx={{ fontWeight: 600 }}>
          本日のオファー
        </Typography>
        <Typography variant="body2" sx={{ mb: 2, color: 'text.secondary' }}>
          機能の先行体験ができます。
        </Typography>
        <Button variant="contained" size="small" fullWidth>
          Got it
        </Button>
      </CardContent>
    </Card>
  );
}
