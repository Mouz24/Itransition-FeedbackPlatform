import React, { useEffect, useState, useRef } from 'react';
import { Review } from './Entities';
import axiosInstance from './AxiosInstance';
import { Avatar, Box, CircularProgress, Divider, Rating, Typography } from '@mui/material';
import { ReviewsProps } from './Props/ReviewsProps';
import { Link } from 'react-router-dom';
import { canDoReviewManipulations, getAvatarContent } from './UserContext';
import signalRArtworkService from './SignalRArtworkService';
import ReviewItem from './ReviewItem';
import signalRLikeService from './SignalRLikeService';

const AllReviews: React.FC<ReviewsProps> = ({ loggedInUserId, tagIds, isLoading, setIsLoading }) => {
  const [reviews, setReviews] = useState<{ [key: string]: Review }>({});
  const pageNumber = useRef<number>(1);
  const hasMoreReviews = useRef<boolean>(true);
  const containerRef = useRef<HTMLDivElement | null>(null);
  const artworkHubConnection = signalRArtworkService.getConnection();
  const likeHubConnection = signalRLikeService.getConnection();

  useEffect(() => {
    if (artworkHubConnection) {
      artworkHubConnection.on('RatedArtwork', () => {
        fetchReviews();
      });
    
    if (likeHubConnection) {
      likeHubConnection.on('LikedReview', () => {
        fetchReviews();
      });
    }
    }
  }, [artworkHubConnection, likeHubConnection]);

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
    if (isLoading || !hasMoreReviews) return;
  
    setIsLoading(true);
  
    const queryParams = [];
  
    if (loggedInUserId) {
      queryParams.push(`userid=${loggedInUserId}`);
    }
  
    if (tagIds.length > 0) {
      const idsQueryParam = tagIds.map(tag => `tagids=${tag}`).join('&');
      queryParams.push(idsQueryParam);
    }
  
    queryParams.push(`pageNumber=${pageNumber.current}`);
  
    const query = queryParams.join('&');
  
    const response = await axiosInstance.get(`reviews${query ? `?${query}` : ''}`);
    console.log(response.data.length);
    if (response.data.length > 0) {
      setReviews(prevReviews => {
        const updatedReviews = { ...prevReviews };
  
        response.data.forEach((newReview: Review) => {
          updatedReviews[newReview.id] = newReview;
        });
  
        return updatedReviews;
      }); 
    } 
    
    if (response.data.length < 4) {
      hasMoreReviews.current = false;
    } else {
      pageNumber.current += 1;
    }
  
    setIsLoading(false);
  };
  
  const handleScroll = () => {
    if (
      containerRef.current &&
      containerRef.current.scrollTop + containerRef.current.clientHeight >= containerRef.current.scrollHeight &&
      hasMoreReviews.current
    ) {
      fetchReviews();
    }
  };

  return (
    <div ref={containerRef} style={{ overflowY: 'scroll', height: '600px'}}>
      {Object.values(reviews).map((review) => (
        <ReviewItem key={review.id} review={review} loggedInUserId={loggedInUserId}/>
      ))}
      {isLoading && <CircularProgress />}
    </div>
  );
};

export default AllReviews;
