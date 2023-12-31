import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import Menu from '@mui/material/Menu';
import Container from '@mui/material/Container';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import Tooltip from '@mui/material/Tooltip';
import MenuItem from '@mui/material/MenuItem';
import { UserContext, useUserContext, getAvatarContent } from './UserContext';
import { deepPurple } from '@mui/material/colors';
import LogoutButton from './LogOut';
import { Link } from 'react-router-dom';
import { User } from '../props/Entities';
import { Switch } from '@mui/material';
import LightModeIcon from '@mui/icons-material/LightMode';
import NightlightIcon from '@mui/icons-material/Nightlight';

const Header: React.FC<{ isLogin: boolean; setIsLogin: React.Dispatch<React.SetStateAction<boolean>> }> = ({ isLogin, setIsLogin }) => {
  const [anchorElUser, setAnchorElUser] = React.useState<null | HTMLElement>(null);
  const { loggedInUser, setLoggedInUser, toggleDarkMode } = useUserContext();

  const handleOpenUserMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorElUser(event.currentTarget);
  };

  const handleCloseUserMenu = () => {
    setAnchorElUser(null);
  };

  return (
    <AppBar position="static" sx={{background: loggedInUser?.isDarkMode ? '#009688' : '#81c784'}}>
        <Toolbar>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', width: '100%' }}>
            <div>
              <Link to={'/'} style={{textDecoration: 'none', color: 'white'}}>
                <Typography variant="h6" component="div" style={{ flexGrow: 1}} fontFamily={'fantasy'}>
                  FeedbackFusion
                </Typography>
              </Link>
            </div>
            {loggedInUser ? (
              <Tooltip title="Open settings">
                <div>
                  <IconButton onClick={handleOpenUserMenu} sx={{ p: 0 }}>
                    {getAvatarContent(loggedInUser)}
                  </IconButton>
                  <Menu
                    sx={{ mt: '45px' }}
                    id="menu-appbar"
                    anchorEl={anchorElUser}
                    anchorOrigin={{
                      vertical: 'top',
                      horizontal: 'right',
                    }}
                    keepMounted
                    transformOrigin={{
                      vertical: 'top',
                      horizontal: 'right',
                    }}
                    open={Boolean(anchorElUser)}
                    onClose={handleCloseUserMenu}
                  >
                    <MenuItem onClick={handleCloseUserMenu}>
                      <Link to={`/${loggedInUser.id}/reviews`} style={{textDecoration: 'none'}}>
                        <Typography variant='h6' textAlign="center">Profile</Typography>
                      </Link>
                    </MenuItem>
                    <MenuItem onClick={handleCloseUserMenu}>
                      <Link to={`/avatar-upload`} style={{textDecoration: 'none'}}>
                        <Typography variant='h6' textAlign="center">Avatar</Typography>
                      </Link>
                    </MenuItem>
                    <MenuItem onClick={handleCloseUserMenu}>
                      <LogoutButton />
                    </MenuItem>
                    <MenuItem>
                    <Box display='flex' alignItems="center" justifyContent="flex-end">
                      <LightModeIcon />
                      <Switch
                        checked={loggedInUser.isDarkMode}
                        onChange={toggleDarkMode}
                        color="primary"
                      />
                      <NightlightIcon />
                    </Box>
                    </MenuItem>
                    {loggedInUser.role === 'Administrator' &&
                      <MenuItem onClick={handleCloseUserMenu}>
                        <Link to={`/admin-panel`} style={{textDecoration: 'none'}}>
                          <Typography variant='h6' textAlign="center">Admin</Typography>
                        </Link>
                    </MenuItem>
                    }
                  </Menu>
                </div>
              </Tooltip>
            ) : (
              !isLogin && (
                <Button component={Link} onClick={() => setIsLogin(true)} to="/authorization-page" sx={{color: 'white'}}>
                  Login
                </Button>
              )
            )}
          </Box>
        </Toolbar>
    </AppBar>
  );
};

export default Header;
