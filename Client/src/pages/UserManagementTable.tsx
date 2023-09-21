import React, { useState, useEffect, useContext } from 'react';
import axiosInstance from '../components/AxiosInstance';
import { Box, Checkbox, Table, TableBody, TableCell, TableHead, TableRow, Typography } from '@mui/material';
import UserToolbar from '../components/UserManagementToolbar';
import { Link, useNavigate } from 'react-router-dom';
import TableContainer from '@mui/material/TableContainer';
import TablePagination from '@mui/material/TablePagination';
import { useUserContext } from '../components/UserContext';
import signalRUserService from '../service/SignalRUserService';

interface User {
  id: string;
  username: string;
  email: string;
  role: string;
  lastLoginDate: string;
  registrationDate: string;
  isBlocked: boolean;
}

const UserManagementTable: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [selectedUserIds, setSelectedUserIds] = useState<string[]>([]);
  const navigate = useNavigate();
  const { loggedInUser } = useUserContext();
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(5);
  const loggedInUserData = users.find((user) => user.username === loggedInUser?.userName);
  const userHubConnection = signalRUserService.getConnection();

  const handleChangePage = (event: React.MouseEvent<HTMLButtonElement> | null, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const fetchUsers = async () => {
    try {
      const response = await axiosInstance.get<User[]>(`users?loggedInUserId=${loggedInUser?.id}`);
      setUsers(response.data);
    } catch (error) {
      console.error('Error fetching users:', error);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, [selectedUserIds]);

  useEffect (() => {
    if (userHubConnection) {
      userHubConnection.on('UserBlocked', () => {
        fetchUsers();
      });

      userHubConnection.on('UserUnblocked', () => {
        fetchUsers();
      });

      userHubConnection.on('UserRoleChange', () => {
        fetchUsers();
      });
    }
  })

  const handleLogout = async () => {
    localStorage.clear();
    navigate('/');
    window.location.reload();
  };

  const toggleUserBlockStatus = async () => {
    try {
      if (!loggedInUser || loggedInUserData?.isBlocked)
      {
        handleLogout();
      } else {
        signalRUserService.BlockUser(selectedUserIds);
        
        setSelectedUserIds([]);
      }
    } catch (error) {
        console.error('Error updating user block status:', error);
    }
  };

  const toggleUserUnblockStatus = async () => {
    try {
      if (!loggedInUser || loggedInUserData?.isBlocked)
      {
        handleLogout();
      } else {
        signalRUserService.UnblockUser(selectedUserIds);
        setSelectedUserIds([]);
      }} catch (error) {
        console.error('Error updating user block status:', error);
    }
  };

  const toggleUserDeleteStatus = async () => {
    try {
      if (!loggedInUser || loggedInUserData?.isBlocked) {
        handleLogout();
      } else {
          await axiosInstance.delete(`users/delete`, {
            data: selectedUserIds,
          });

          const updatedUsers = users.filter((user) => !selectedUserIds.includes(user.id));
          setUsers(updatedUsers);
          setSelectedUserIds([]);
        }
        
      fetchUsers();
    } catch (error) {
      console.error('Error updating user delete status:', error);
    }
  };

  const toggleUserRole = async () => {
    try {
      if (!loggedInUser || loggedInUserData?.isBlocked)
      {
        handleLogout();
      } else {
        signalRUserService.MakeUserAdmin(selectedUserIds);
        setSelectedUserIds([]);
      }} catch (error) {
        console.error('Error updating user block status:', error);
    }
  };

  const handleSelectUser = (userId: string) => {
    setSelectedUserIds(prevSelectedIds => {
      if (prevSelectedIds.includes(userId)) {
        return prevSelectedIds.filter(id => id !== userId);
      } else {
        return [...prevSelectedIds, userId];
      }
    });
  };

  const renderUserRows = () => {
    const startIndex = page * rowsPerPage;
    const endIndex = startIndex + rowsPerPage;
    const visibleUsers = users.slice(startIndex, endIndex);
  
    return visibleUsers.map((user) => (
    <TableRow key={user.id}>
      <TableCell padding="checkbox">
        <Checkbox
          checked={selectedUserIds.includes(user.id)}
          onChange={() => handleSelectUser(user.id)}
        />
      </TableCell>
      <TableCell>
        <Link to={`/${user.id}/reviews`} style={{textDecoration: 'none'}}>
          <Typography variant='h6'>{user.username}</Typography>
        </Link>
      </TableCell>
      <TableCell>
        <Typography variant='h6'>{user.email} </Typography>
      </TableCell>
      <TableCell>
        <Typography variant='h6'>{user.role} </Typography>
      </TableCell>
      <TableCell>
        <Typography variant='h6'>{user.lastLoginDate} </Typography>
      </TableCell>
      <TableCell>
        <Typography variant='h6'>{user.registrationDate} </Typography>
      </TableCell>
      <TableCell>
        <Typography variant='h6'>{user.isBlocked ? 'Blocked' : 'Active'} </Typography>
      </TableCell>
    </TableRow>
  ));
  }

  return (
    <Box marginTop={'10px'}>
      <UserToolbar
        selectedUserIds={selectedUserIds}
        toggleUserBlockStatus={toggleUserBlockStatus}
        toggleUserUnblockStatus={toggleUserUnblockStatus}
        toggleUserDeleteStatus={toggleUserDeleteStatus}
        toggleUserRole={toggleUserRole}
      />
      <TableContainer>
        <Table
          aria-labelledby="tableTitle"
          size="small"
          style={{ height: 300 }}
        >
          <TableHead>
            <TableRow>
              <TableCell></TableCell>
              <TableCell>Username</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Role</TableCell>
              <TableCell>Last Login Time</TableCell>
              <TableCell>Registration Time</TableCell>
              <TableCell>Status</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {renderUserRows()}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        rowsPerPageOptions={[5, 10, 25]}
        component="div"
        count={users.length}
        rowsPerPage={rowsPerPage}
        page={page}
        onPageChange={handleChangePage}
        onRowsPerPageChange={handleChangeRowsPerPage}
      />
      </Box>
  );
};

export default UserManagementTable;