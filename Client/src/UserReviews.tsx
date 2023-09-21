import React, { useEffect, useState } from 'react';
import axiosInstance from './AxiosInstance';
import { Link, Route, Routes, useParams } from 'react-router-dom';
import { Review, User } from './Entities';
import { Avatar, Box, Button, CircularProgress, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TableSortLabel, TextField, Typography } from '@mui/material';
import { UserReviewsProps } from './Props/UserReviewsProps';
import { canDoReviewManipulations, getAvatarContent, useUserContext } from './UserContext';

const UserReviews: React.FC = () => {
  const [userReviews, setUserReviews] = useState<Review[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortBy, setSortBy] = useState<keyof Review>('dateCreated');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');
  const { userId } = useParams<{ userId: string }>();
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const { loggedInUser } = useUserContext();

  useEffect(() => {
    fetchUserReviews();
  }, [userId, sortBy, sortOrder]);

  useEffect(() => {
    fetchUserData();
  }, [userId]);

  const fetchUserData = async () => {
    try {
      const response = await axiosInstance.get<User>(`/users/${userId}`);
      setUser(response.data);
    } catch (error) {
      console.error('Error fetching user data:', error);
    }
  };

  const fetchUserReviews = async () => {
    if (isLoading) return;

    setIsLoading(true);
    try {
      const response = await axiosInstance.get(`/review/${userId}`);
      setIsLoading(false);
      const sortedReviews = sortReviews(response.data, sortBy, sortOrder);
      setUserReviews(sortedReviews);
    } catch (error) {
      console.error('Error fetching user reviews:', error);
    }
  };

  const sortReviews = (reviews: Review[], sortBy: keyof Review, sortOrder: 'asc' | 'desc') => {
    return reviews.sort((a, b) => {
      if (sortOrder === 'asc') {
        return a[sortBy] > b[sortBy] ? 1 : -1;
      } else {
        return a[sortBy] < b[sortBy] ? 1 : -1;
      }
    });
  };

  const handleSort = (column: keyof Review) => {
    if (sortBy === column) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(column);
      setSortOrder('asc');
    }
  };

  const handleSearch = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(event.target.value);
  };

  const filteredReviews = userReviews.filter((review) =>
    review.title.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <>
      <div style={{ display: 'flex', alignItems: 'center', marginTop: '16px', marginBottom: '16px', gap: '7px' }}>
        {getAvatarContent(user)}
        <Typography variant="h5">{user?.userName}</Typography>
        <Typography variant="h6" sx={{display: 'flex', marginLeft: 'auto'}}>
          Likes: {user?.likes}
        </Typography>
      </div>
      <TextField
        label="Search"
        variant="outlined"
        fullWidth
        onChange={handleSearch}
        value={searchTerm}
        style={{ marginBottom: '16px' }}
      />
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'title'}
                  direction={sortOrder}
                  onClick={() => handleSort('title')}
                >
                  Title
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'mark'}
                  direction={sortOrder}
                  onClick={() => handleSort('mark')}
                >
                  Mark
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'dateCreated'}
                  direction={sortOrder}
                  onClick={() => handleSort('dateCreated')}
                >
                  Date Created
                </TableSortLabel>
              </TableCell>
              <TableCell>
                <TableSortLabel
                  active={sortBy === 'likes'}
                  direction={sortOrder}
                  onClick={() => handleSort('likes')}
                >
                  Likes
                </TableSortLabel>
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
      {isLoading ? (
        <TableRow>
          <TableCell colSpan={4} align="center">
            <CircularProgress />
          </TableCell>
        </TableRow>
      ) : (
        filteredReviews.map((review) => (
          <TableRow key={review.id}>
            <TableCell>
              <Link to={`/${userId}/reviews/${review.id}`} style={{textDecoration: 'none'}}>
                <Typography variant='h6'>{review.title}</Typography>
              </Link>
            </TableCell>
            <TableCell>{review.mark}</TableCell>
            <TableCell>{review.dateCreated}</TableCell>
            <TableCell>{review.likes}</TableCell>
          </TableRow>
        ))
      )}
    </TableBody>
        </Table>
    </TableContainer>
    {canDoReviewManipulations(loggedInUser, userId) && (
        <Button
          variant="contained"
          color="primary"
          component={Link}
          to={`/${userId}/add-review`}
          style={{ marginBottom: '16px' }}
        >
          Add Review
        </Button>
    )}
    </>
  );
};

export default UserReviews;
