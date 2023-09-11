import React, { useEffect, useState, useRef } from 'react';
import { Review } from './Entities';
import axiosInstance from './AxiosInstance';
import { Avatar, Box, CircularProgress, Rating, Typography } from '@mui/material';
import signalRService from './SignalRService';
import { ReviewsProps } from './Props/ReviewsProps';
import { Link } from 'react-router-dom';
import { canDoReviewManipulations, getAvatarContent } from './UserContext';

const AllReviews: React.FC<ReviewsProps> = ({ loggedInUserId, tagIds, isLoading, setIsLoading }) => {
  const [reviews, setReviews] = useState<Review[]>([]);
  const [pageNumber, setPageNumber] = useState<number>(1);
  const containerRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    fetchReviews();
  }, [loggedInUserId, tagIds]);

  useEffect(() => {
    if (containerRef.current) {
      containerRef.current.addEventListener('scroll', handleScroll);
    }

    return () => {
      if (containerRef.current) {
        containerRef.current.removeEventListener('scroll', handleScroll);
      }
    }
  }, []);

  const fetchReviews = async () => {
    if (isLoading) return;

    setIsLoading(true);

    const queryParams = [];

    if (loggedInUserId) {
      queryParams.push(`userid=${loggedInUserId}`);
    }

    if (tagIds.length > 0) {
      const idsQueryParam = tagIds.map(tag => `tagids=${tag}`).join('&');
      queryParams.push(idsQueryParam);
    }

    queryParams.push(`pageNumber=${pageNumber}`);

    const query = queryParams.join('&');

    const response = await axiosInstance.get(`review${query ? `?${query}` : ''}`)

    if (response.data.length > 0) {
      setReviews(prevReviews => [...prevReviews, ...response.data]);
      setPageNumber(prevPageNumber => prevPageNumber + 1);
    }

    setIsLoading(false);
  };

  const handleScroll = () => {
    if (
      containerRef.current &&
      containerRef.current.scrollTop + containerRef.current.clientHeight >= containerRef.current.scrollHeight
    ) {
      fetchReviews();
    }
  };

  return (
    <div ref={containerRef} style={{ overflowY: 'scroll', height: '400px' }}>
      {reviews.map((review) => (
        <div key={review.id} style={{
          border: '2px solid #ccc',
          borderRadius: '10px',
          padding: '16px',
          margin: '16px 0',
          display: 'flex',
          alignItems: 'center',
        }}>
          <Link to={`/${review.user.id}/reviews`}>
            {getAvatarContent(review.user)}
          </Link>
          <Link to={`/${review.user.id}/reviews`}>
            <Typography variant="body2">{review.user.username}</Typography>
          </Link>
          <Typography variant="body2">{review.user.likes}</Typography>
          <div style={{ flex: 1 }}>
          <Link to={`/${review.user.id}/reviews/${review.id}`}>
            <Typography variant="h6">{review.title}</Typography>
          </Link>
            <Typography variant="body2">{review.artwork.name}</Typography>
            <Box display="flex" alignItems="center">
            {loggedInUserId ? (
              <Rating
              name={`rating-${review.artwork.name}`}
              value={review.artwork.rate}
              onChange={(event, newValue) => signalRService.RateArtwork(review.artwork.id, loggedInUserId || '', newValue)}
              />
            ) : (
              <Rating
                name={`rating-${review.artwork.name}`}
                value={review.artwork.rate}
                readOnly
              />
            )}
            <Typography variant="body2" style={{ marginLeft: '4px' }}>
              {review.artwork.rate}
            </Typography>
            </Box>
          </div>
          <div>
            <Typography variant="body2">{review.group.name}</Typography>
          </div>
        </div>
      ))}
      { isLoading && <CircularProgress />}
    </div>
  );
};

export default AllReviews;
