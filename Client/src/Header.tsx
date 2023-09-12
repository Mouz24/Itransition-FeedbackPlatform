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
import { User } from './Entities';

const Header: React.FC<{ isLogin: boolean; setIsLogin: React.Dispatch<React.SetStateAction<boolean>> }> = ({ isLogin, setIsLogin }) => {
  const [anchorElUser, setAnchorElUser] = React.useState<null | HTMLElement>(null);
  const { loggedInUser, setLoggedInUser } = useUserContext();

  const handleOpenUserMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorElUser(event.currentTarget);
  };

  const handleCloseUserMenu = () => {
    setAnchorElUser(null);
  };

  return (
    <AppBar position="static" sx={{background: '#81c784'}}>
        <Toolbar>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', width: '100%' }}>
            <div>
              <Typography variant="h6" component="div" style={{ flexGrow: 1}}>
                Your Sign
              </Typography>
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
                        <Typography textAlign="center">Profile</Typography>
                      </Link>
                    </MenuItem>
                    <MenuItem onClick={handleCloseUserMenu}>
                      <LogoutButton />
                    </MenuItem>
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
