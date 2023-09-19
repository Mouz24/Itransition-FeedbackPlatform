import React, { useEffect, useState } from 'react';
import './UserReview.css';
import axiosInstance from './AxiosInstance';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { Review, ReviewImage } from './Entities';
import { Avatar, Box, Button, Card, CardContent, CardHeader, CircularProgress, Divider, IconButton, List, ListItem, ListItemText, Rating, TextField, Typography } from '@mui/material';
import { canDoReviewManipulations, getAvatarContent, useUserContext } from './UserContext';
import { CommentDTO } from './EntitiesDTO';
import { DeleteOutline } from '@mui/icons-material';
import ImageList from '@mui/material/ImageList';
import ImageListItem from '@mui/material/ImageListItem';
import ImageGallery from 'react-image-gallery';
import 'react-image-gallery/styles/css/image-gallery.css';
import signalRCommentService from './SignalRCommentService';
import signalRLikeService from './SignalRLikeService';
import signalRArtworkService from './SignalRArtworkService';
import DeleteIcon from '@mui/icons-material/Delete';
import EditRoundedIcon from '@mui/icons-material/EditRounded';
import ReviewItem from './ReviewItem';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import FavoriteIcon from '@mui/icons-material/Favorite';

const UserReview: React.FC = () => {
  const { userId, reviewId } = useParams<{ userId: string, reviewId: string }>();
  const [review, setReview] = useState<Review | null>(null);
  const [connectedReviews, setConnectedReviews] = useState<Review[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const { loggedInUser } = useUserContext();
  const navigate = useNavigate();
  const [imageIndex, setImageIndex] = useState<number>(0);
  const [isGalleryOpen, setIsGalleryOpen] = useState<boolean>(false);
  const [comment, setComment] = useState<CommentDTO>({
    text: '',
    reviewId: reviewId,
    userId: loggedInUser?.id
  });
  const commentHubConnection = signalRCommentService.getConnection();
  const likeHubConnection = signalRLikeService.getConnection();
  const artworkHubConnection = signalRArtworkService.getConnection();

  useEffect(() => {
    if (artworkHubConnection) {
      artworkHubConnection.on('RatedArtwork', () => {
        fetchReview();
      });
    }

    if (likeHubConnection) {
      likeHubConnection.on('LikedReview', () => {
        fetchReview();
      });
    }

    if (commentHubConnection) {
      commentHubConnection.on('ReceiveComment', () => {
        fetchReview();
      });

      commentHubConnection.on('RemoveComment', () => {
        fetchReview();
      });
    }
  }, [artworkHubConnection, likeHubConnection, commentHubConnection]);

  useEffect(() => {
    fetchReview();
    fetchConnectedReviews();
  }, [reviewId]);

  const fetchReview = async () => {
    if (isLoading) return;

    setIsLoading(true);
    try {
        const response = await axiosInstance.get(`review/${userId}/${reviewId}?loggedInUserId=${loggedInUser?.id}`);
        setReview(response.data);
        console.log(response.data);
        setIsLoading(false);
    } catch (error) {
        console.error('Error fetching user data:', error);
    }
  }

  const fetchConnectedReviews = async () => {
    if (isLoading) return;

    setIsLoading(true);
    try {
        const response = await axiosInstance.get(`review/${userId}/${reviewId}/connected-reviews`);
        setConnectedReviews(response.data);
        setIsLoading(false);
    } catch (error) {
        console.error('Error fetching user data:', error);
    }
  }

  const handleCommentInputChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
    setComment({
      ...comment,
      text: event.target.value,
    });
  };

  const handleDelete = async() => {
    try {
      await axiosInstance.delete(`review/${userId}/${reviewId}`);
      navigate(`/${userId}/reviews`);
    } catch (error) {
      console.error('Error fetching user data:', error);
    }
  }

  const handleLikeReview = () => {
    signalRLikeService.LikeReview(review?.id, userId, loggedInUser?.id);
  };

  const images = review?.reviewImages?.map((image: ReviewImage) => ({
    original: image.imageUrl,
    thumbnail: image.imageUrl,
  })) || [];

  return (
    <Box>
      { isLoading && <CircularProgress />}
      {canDoReviewManipulations(loggedInUser, userId) && (
        <Box sx={{display: 'flex'}}>
          <Button
          component={Link}
          to={`/${userId}/reviews/${reviewId}/edit`}
          >
            <EditRoundedIcon color='action' />
          </Button>
          <Button onClick={handleDelete}>
            <DeleteIcon color='error' />
          </Button>
        </Box>
      )}
      {review && (
        <Card>
          <CardHeader
            avatar={getAvatarContent(review.user)}
            title={review.user.userName}
            subheader={review.dateCreated}
          />
          <Divider />
          <CardContent sx={{display: 'flex', flexDirection: 'column', alignItems: 'center'}}>
            <Typography variant="h5" sx={{ fontWeight: 'bold'}}>
              {review.title}
            </Typography>
            <div dangerouslySetInnerHTML={{ __html: review.text }} />
            <List sx={{display: 'flex', flexDirection: 'column', alignItems: 'center'}}>
              <ListItem>
                <Typography variant="body1" style={{ fontWeight: 'bold' }}>
                  Mark: {review.mark}
                </Typography>
              </ListItem>
              <ListItem>
                <Typography variant="body1" style={{ fontWeight: 'bold' }}>
                  Artwork Name: {review.artwork.name}
                </Typography>
              </ListItem>
              <ListItem>
                <Box display="flex" alignItems="center">
                  {loggedInUser?.id ? (
                    <Rating
                    name={`rating-${review.artwork.name}`}
                    value={review.artwork.rate}
                    onChange={(event, newValue) => signalRArtworkService.RateArtwork(review.artwork.id, loggedInUser.id || '', newValue)}
                    />
                  ) : (
                    <Rating
                      name={`rating-${review.artwork.name}`}
                      value={review.artwork.rate}
                      readOnly
                    />
                  )}
                  <Typography variant="h6" style={{ marginLeft: '4px' }}>
                    ({review.artwork.rate})
                  </Typography>
                </Box>
              </ListItem>
              <ListItem>
                <Typography variant="body1" style={{ fontWeight: 'bold' }}>
                  Group: {review.group.name}
                </Typography>
              </ListItem>
              <ListItem>
                {review.tags.length > 0 && 
                  <Typography variant="body1" style={{ fontWeight: 'bold' }}>
                  Tags: {review.tags.map((tag) => `#${tag.value} ` )}
                </Typography>
                }
              </ListItem>
              </List>
              </CardContent>
              <Divider />
              <ImageList cols={3} sx={{display: 'flex', justifyContent: 'center'}}>
                {review.reviewImages?.map((image: ReviewImage, index: number) => (
                  <ImageListItem
                    key={index}
                    onClick={() => {
                      setImageIndex(index);
                      setIsGalleryOpen(true);
                    }}
                    sx={{width: '200px', ":hover": {cursor: 'pointer'}}}
                  >
                    <img
                      src={image.imageUrl}
                      alt={`${index + 1}`}
                    />
                  </ImageListItem>
                ))}
              </ImageList>
              {loggedInUser && (
                <Button
                onClick={handleLikeReview}
                className='pulse-icon'
                style={{
                  display: 'flex',
                  alignItems: 'center',
                  color: review.isLikedByUser ? 'red' : 'black',
                  transition: 'color 0.3s',
                  fontSize: '1.5rem'
                }}
                startIcon={
                  review.isLikedByUser ? (
                    <FavoriteIcon fontSize='large' style={{ fontSize: '2rem' }}/>
                  ) : (
                    <FavoriteBorderIcon fontSize='large' style={{ fontSize: '2rem' }}/>
                  )
                }
              >
                {review.likes}
              </Button>
              )}
              {connectedReviews.length > 0 && (
                <Box>
                  <Divider />
                  <CardContent>
                    <Typography variant="h6" gutterBottom>
                      Connected Reviews
                    </Typography>
                    <List>
                      {connectedReviews?.map((connectedReview : Review) => (
                        <ReviewItem key={connectedReview.id} review={connectedReview} loggedInUserId={loggedInUser?.id} />
                      ))}
                    </List>
                  </CardContent>
                </Box>
              )}
              <Divider />
        </Card>
      )}
      {review?.comments?.map((comment) => (
        <Box key={comment.id} sx={{display: 'flex', justifyContent: 'center', alignItems: 'center', marginTop: '10px'}}>
        <Box sx={{width: '500px'}}>
          <Card>
            <CardContent>
              <Box sx={{display: 'flex', alignItems: 'center'}}>
                <CardHeader
                  avatar={getAvatarContent(comment.user)}
                  title={comment.user.userName}
                  subheader={comment.dateCreated}
                />
                <Box sx={{display: 'flex', marginLeft: 'auto'}}>
                  {(canDoReviewManipulations(loggedInUser, userId) || comment.user.id === loggedInUser?.id) && (
                    <IconButton
                      aria-label="delete-comment"
                      color="inherit"
                      onClick={() => signalRCommentService.RemoveComment(comment.id, review.id)}
                    >
                      <DeleteOutline />
                    </IconButton>
                  )}
                </Box>
              </Box>
              <Typography variant="body1" style={{ textAlign: 'center' }}>
                {comment.text}
              </Typography>
            </CardContent>
          </Card>
        </Box>
        </Box>
      ))}
      {loggedInUser && (
        <Box sx={{ display:'flex', flexDirection:'column', alignItems:'center', marginTop:'10px' }}>
          <TextField
            variant="outlined"
            label="Add a Comment"
            multiline
            rows={4}
            value={comment.text}
            onChange={handleCommentInputChange}
            sx={{width: '300px'}}
          />
          <Box>
            <Button
              variant="contained"
              color="success"
              onClick={() => { 
                signalRCommentService.LeaveComment(comment);
                setComment({
                  ...comment,
                  text: '',
                });
              }}
            >
              Add Comment
            </Button>
          </Box>
      </Box>              
      )}
      {isGalleryOpen && (
        <Box
        sx={{
          position: 'fixed',
          top: 0,
          left: 0,
          width: '100%',
          height: '100%',
          background: 'rgba(0, 0, 0, 0.8)',
          zIndex: 1000,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
        }}
        >
          <ImageGallery
                items={images}
                showPlayButton={false}
                showFullscreenButton={false}
                startIndex={imageIndex}
          />
        <Button onClick={() => setIsGalleryOpen(false)} 
          sx={{
            position: 'absolute',
            top: '20px',
            right: '20px',
            zIndex: 1001,
          }}
        >
          Close Gallery
        </Button>
      </Box>
      )}
    </Box>
  );
};

export default UserReview;
