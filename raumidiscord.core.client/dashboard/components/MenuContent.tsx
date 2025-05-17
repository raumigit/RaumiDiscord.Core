import AnalyticsRoundedIcon from '@mui/icons-material/AnalyticsRounded';
import AssignmentRoundedIcon from '@mui/icons-material/AssignmentRounded';
import HelpRoundedIcon from '@mui/icons-material/HelpRounded';
import HomeRoundedIcon from '@mui/icons-material/HomeRounded';
import InfoRoundedIcon from '@mui/icons-material/InfoRounded';
import PeopleRoundedIcon from '@mui/icons-material/PeopleRounded';
import SettingsRoundedIcon from '@mui/icons-material/SettingsRounded';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import Stack from '@mui/material/Stack';

const mainListItems = [
  { text: 'Home', icon: <HomeRoundedIcon />, url:'#' },
  { text: 'アナリティクス', icon: <AnalyticsRoundedIcon />, url:'#' },
  { text: 'クライアント', icon: <PeopleRoundedIcon />, url:'#' },
  { text: 'タスク', icon: <AssignmentRoundedIcon />, url:'#' },
  { text: 'YoHoTool', icon: <AssignmentRoundedIcon />, url:'./tools/index.html' }
];

const secondaryListItems = [
  { text: '設定', icon: <SettingsRoundedIcon /> },
  { text: 'About', icon: <InfoRoundedIcon /> },
  { text: 'フィードバック', icon: <HelpRoundedIcon /> },
];

export default function MenuContent() {
  return (
    <Stack sx={{ flexGrow: 1, p: 1, justifyContent: 'space-between' }}>
      <List dense>
        {mainListItems.map((item, index) => (
          <ListItem key={index} disablePadding sx={{ display: 'block' }}>
            <ListItemButton selected={index === 0} href={item.url}>
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
      <List dense>
        {secondaryListItems.map((item, index) => (
          <ListItem key={index} disablePadding sx={{ display: 'block' }}>
            <ListItemButton>
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Stack>
  );
}
