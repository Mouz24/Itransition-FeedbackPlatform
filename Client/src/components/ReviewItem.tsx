import React from 'react';
import { Box, Divider, Rating, Typography } from '@mui/material';
import { Link } from 'react-router-dom';
import signalRArtworkService from '../service/SignalRArtworkService';
import { Review } from '../props/Entities';
import { getAvatarContent } from './UserContext';
import { ReviewItemProps } from '../props/ReviewItemProps';
import { deepPurple } from "@mui/material/colors";

const ReviewItem: React.FC<ReviewItemProps> = ({ review, loggedInUserId }) => {
  return (
    <Box key={review.id} sx={{
      border: '2px solid #ccc',
      borderRadius: '10px',
      padding: '16px',
      margin: '16px 0',
      width: "auto"
    }}>
      <Box sx={{ display: 'flex', flexWrap: "wrap", gap: '10px' }}>
        <Link to={`/${review.user.id}/reviews`} style={{textDecorationLine: 'none'}}>
          {getAvatarContent(review.user)}
        </Link>
        <Link to={`/${review.user.id}/reviews`} style={{textDecorationLine: 'none', color: `${deepPurple[500]}`}}>
          <Typography variant="h6">{review.user.userName}</Typography>
        </Link>
        <Box sx={{marginLeft: 'auto'}}>
        <Typography variant="h6">Likes: {review.likes}</Typography>
        </Box>
      </Box>
      <Divider />
      <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center' }}>
        <Link to={`/${review.user.id}/reviews/${review.id}`} 
          style={{textDecorationLine: 'none'}}>
            <Typography variant="h5" sx={{fontWeight: 'bold'}}>{review.title}</Typography>
        </Link>
        <Box sx={{ display: 'flex', alignItems: 'center' }}>
          <Typography variant="h6" sx={{ marginRight: '7px' }}>{review.artwork.name}</Typography>
          {loggedInUserId ? (
            <Rating
              name={`rating-${review.artwork.name}`}
              value={review.artwork.rate}
              onChange={(event, newValue) => signalRArtworkService.RateArtwork(review.artwork.id, loggedInUserId || '', newValue)}
            />
          ) : (
            <Rating
              name={`rating-${review.artwork.name}`}
              value={review.artwork.rate}
              readOnly
            />
          )}
          <Typography variant="body2" style={{ marginLeft: '4px' }}>
            ({review.artwork.rate})
          </Typography>
        </Box>
        <Typography variant="h6"><span style={{fontWeight: 'bold'}}>Category:</span> {review.group.name}</Typography>
        {review.tags.length > 0 && 
          <Typography variant="body1" style={{ fontWeight: 'bold' }}>
            Tags: {review.tags.map((tag) => `#${tag.value} ` )}
          </Typography>}
      </Box>
    </Box>
  );
}

export default ReviewItem;
