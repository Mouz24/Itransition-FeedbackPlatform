import React, { useEffect, useState, useRef } from 'react';
import { Review } from '../props/Entities';
import axiosInstance from './AxiosInstance';
import { Avatar, Box, CircularProgress, Divider, Rating, Typography } from '@mui/material';
import { ReviewsProps } from '../props/ReviewsProps';
import { Link } from 'react-router-dom';
import { canDoReviewManipulations, getAvatarContent } from './UserContext';
import signalRArtworkService from '../service/SignalRArtworkService';
import ReviewItem from './ReviewItem';
import isEqual from 'lodash/isEqual';
import signalRLikeService from '../service/SignalRLikeService';

const Reviews: React.FC<ReviewsProps> = ({goal, loggedInUserId, tagIds, isLoading, setIsLoading }) => {
  const [reviews, setReviews] = useState<Review[]>([]);
  const pageNumber = useRef<number>(1);
  const hasMoreReviews = useRef<boolean>(false);
  const containerRef = useRef<HTMLDivElement | null>(null);
  const artworkHubConnection = signalRArtworkService.getConnection();
  const likeHubConnection = signalRLikeService.getConnection();
  const useEffectTriggered = useRef(false);
  const artworkRateUpdate = useRef<boolean>(false);
  const tagIdsRef = useRef<number[]>(tagIds);

  useEffect(() => {
    if (artworkHubConnection) {
      artworkHubConnection.on('RatedArtwork', () => {
        const currentPageNumber = pageNumber.current;
        artworkRateUpdate.current = true;
        
        for (let i = 1; i <= currentPageNumber; i++) {
          pageNumber.current = i;
          fetchReviews(true, tagIdsRef.current);
        }

        artworkRateUpdate.current = false;
      });
    
    if (likeHubConnection) {
      likeHubConnection.on('LikedReview', () => {
        fetchReviews(artworkRateUpdate.current, tagIds);
      });
    }
    }
  }, [artworkHubConnection, likeHubConnection]);

  useEffect(() => {
    pageNumber.current = 1;
    hasMoreReviews.current = true;
    tagIdsRef.current = tagIds;
    setReviews([]);
    fetchReviews(artworkRateUpdate.current, tagIds);
  }, [tagIds]);

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

  const fetchReviews = async (isArtworkRateUpdated: boolean, currentTagIds: number[]) => {
    if (isLoading || !hasMoreReviews.current) return;
    console.log(currentTagIds);

    setIsLoading(true);
    useEffectTriggered.current = true;
  
    const queryParams = [];
  
    if (currentTagIds.length > 0) {
      const idsQueryParam = currentTagIds.map(tag => `tagids=${tag}`).join('&');
      queryParams.push(idsQueryParam);
    }
  
    queryParams.push(`pageNumber=${pageNumber.current}`);
  
    const query = queryParams.join('&');
  
    const response = await axiosInstance.get(`reviews${goal === 'all' ? '' : `/${goal}`}${query ? `?${query}` : ''}`);
    if (response.data.length > 0) {
      setReviews(prevReviews => {
        const updatedReviews = [...prevReviews];
  
        response.data.forEach((newReview: Review) => {
          const existingReviewIndex = updatedReviews.findIndex(review => review.id === newReview.id);
  
          if (existingReviewIndex !== -1) {
            updatedReviews[existingReviewIndex] = newReview;
          } else {
            updatedReviews.push(newReview);
          }
        });
  
        return updatedReviews;
      });

      if (!isArtworkRateUpdated) {
        pageNumber.current = pageNumber.current + 1;
      }
    } else {
      hasMoreReviews.current = false;
    }
  
    setIsLoading(false);
    useEffectTriggered.current = false;
  };
  
  const handleScroll = () => {
    if (
      containerRef.current &&
      containerRef.current.scrollTop + containerRef.current.clientHeight >= containerRef.current.scrollHeight && 
      hasMoreReviews.current && 
      !useEffectTriggered.current
    ) {
      fetchReviews(artworkRateUpdate.current, tagIdsRef.current);
    }
  };

  return (
    <div ref={containerRef} style={{ overflowY: 'scroll', height: '600px'}}>
      {reviews.map((review) => (
        <ReviewItem key={review.id} review={review} loggedInUserId={loggedInUserId}/>
      ))}
      {isLoading && <CircularProgress />}
    </div>
  );
};

export default Reviews;
