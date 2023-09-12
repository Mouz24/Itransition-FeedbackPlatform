import React, { useEffect, useState } from 'react';
import axiosInstance from './AxiosInstance';
import { Link, useParams } from 'react-router-dom';
import { Review, ReviewImage } from './Entities';
import { Avatar, Box, Button, Card, CardContent, CardHeader, Divider, IconButton, List, ListItem, ListItemText, Rating, TextField, Typography } from '@mui/material';
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

const UserReview: React.FC = () => {
  const { userId, reviewId } = useParams<{ userId: string,reviewId: string }>();
  const [review, setReview] = useState<Review | null>(null);
  const [connectedReviews, setConnectedReviews] = useState<Review[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const { loggedInUser } = useUserContext();
  const [imageIndex, setImageIndex] = useState<number>(0);
  const [isGalleryOpen, setIsGalleryOpen] = useState<boolean>(false);
  const [comment, setComment] = useState<CommentDTO>({
    text: '',
    reviewId: review?.id,
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
  }, [artworkHubConnection]);

  useEffect(() => {
    if (likeHubConnection) {
      likeHubConnection.on('LikedReview', () => {
        fetchReview();
      });

      likeHubConnection.on('DislikedReview', () => {
        fetchReview();
      });
    }
  }, [likeHubConnection]);

  useEffect(() => {
    if (commentHubConnection) {
      commentHubConnection.on('ReceiveComment', () => {
        fetchReview();
      });

      commentHubConnection.on('RemoveComment', () => {
        fetchReview();
      });
    }
  }, [commentHubConnection]);

  useEffect(() => {
    fetchReview();
    fetchConnectedReviews();
  }, [reviewId]);

  const fetchReview = async () => {
    if (isLoading) return;

    setIsLoading(true);
    try {
        const response = await axiosInstance.get(`/api/review/${userId}/${reviewId}`);
        setReview(response.data);
        setIsLoading(false);
    } catch (error) {
        console.error('Error fetching user data:', error);
    }
  }

  const fetchConnectedReviews = async () => {
    if (isLoading) return;

    setIsLoading(true);
    try {
        const response = await axiosInstance.get(`/api/review/${userId}/${reviewId}/connected-reviews`);
        setConnectedReviews(response.data);
        setIsLoading(false);
    } catch (error) {
        console.error('Error fetching user data:', error);
    }
  }

  const handleCommentInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setComment((prevFormData) => ({
      ...prevFormData,
      [name]: value
    }));
  };

  const images = review?.reviewImages.map((image: ReviewImage) => ({
    original: image.imageUrl,
    thumbnail: image.imageUrl,
  })) || [];

  return (
    <Box mt={2}>
      {review && (
        <Card>
          <CardHeader
            avatar={getAvatarContent(review.user)}
            title={review.user.username}
            subheader={review.dateCreated}
          />
          <CardContent>
            <Typography variant="h5" component="div">
              {review.title}
            </Typography>
            <Typography variant="body1" color="textSecondary">
              {review.text}
            </Typography>
          </CardContent>
          <Divider />
          <CardContent>
            <List>
              <ListItem>
                <ListItemText primary={`Mark: ${review.mark}`} />
              </ListItem>
              <ListItem>
                <ListItemText primary={`Artwork Name: ${review.artwork.name}`} />
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
                <Typography variant="body2" style={{ marginLeft: '4px' }}>
                  {review.artwork.rate}
                </Typography>
              </Box>
              </ListItem>
              <ListItem>
                <ListItemText primary={`Group: ${review.group.name}`} />
              </ListItem>
              <ImageList cols={3}>
                {review.reviewImages.map((image: ReviewImage, index: number) => (
                  <ImageListItem
                    key={index}
                    onClick={() => {
                      setImageIndex(index);
                      setIsGalleryOpen(true);
                    }}
                  >
                    <img
                      src={image.imageUrl}
                      alt={`${index + 1}`}
                    />
                  </ImageListItem>
                ))}
              </ImageList>
                {loggedInUser && (
                  <ListItem>
                    <Button
                    onClick={() => signalRLikeService.LikeReview(review.id, loggedInUser?.id || '')}
                    variant="contained"
                    style={{
                      backgroundColor: review.isLikedByUser ? 'red' : 'white',
                      color: review.isLikedByUser ? 'white' : 'red',
                    }}
                    >
                    {review.likes} Likes
                    </Button>
                  </ListItem>
                )}
            </List>
          </CardContent>
          {connectedReviews.length > 0 && (
            <>
              <Divider />
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Connected Reviews
                </Typography>
                <List>
                  {connectedReviews.map((connectedReview) => (
                    <ListItem key={connectedReview.id}>
                      <ListItemText>
                        <Link to={`/reviews/${connectedReview.id}`}>{connectedReview.title}</Link>
                      </ListItemText>
                    </ListItem>
                  ))}
                </List>
              </CardContent>
              <Divider />
              {review.comments.map((comment) => (
                <Box key={comment.id} mb={1}>
                  <Card>
                    <CardContent>
                      <Box display="flex" justifyContent="space-between">
                        <Box display="flex" alignItems="center">
                          <Avatar src={comment.user.avatar} alt={comment.user.username} />
                          <Typography variant="subtitle2" style={{ marginLeft: '8px' }}>
                            {comment.user.username}
                          </Typography>
                        </Box>
                      </Box>
                      {canDoReviewManipulations(loggedInUser, userId) && (
                    <IconButton
                      aria-label="delete-comment"
                      color="inherit"
                      onClick={() => signalRCommentService.RemoveComment(comment.id)}
                      style={{ position: 'absolute', top: '8px', right: '8px' }}
                    >
                      <DeleteOutline />
                    </IconButton>
                  )}
                      <Typography variant="body1" style={{ textAlign: 'center' }}>
                        {comment.text}
                      </Typography>
                    </CardContent>
                  </Card>
                </Box>
              ))}
              {loggedInUser && (
                <Box mt={2} p={2} bgcolor="background.default">
                <TextField
                  fullWidth
                  variant="outlined"
                  label="Add a Comment"
                  multiline
                  rows={4}
                  value={comment.text}
                  onChange={handleCommentInputChange}
                />
                <Box mt={2} display="flex" justifyContent="flex-end">
                  <Button
                    variant="contained"
                    color="success"
                    onClick={() => signalRCommentService.LeaveComment(comment)}
                  >
                    Add Comment
                  </Button>
                </Box>
              </Box>
            )}
            </>
          )}
        </Card>
      )}
      {isGalleryOpen && (
        <div>
          <ImageGallery
            items={images}
            showPlayButton={false}
            showFullscreenButton={false}
            startIndex={imageIndex}
          />
          <Button onClick={() => setIsGalleryOpen(false)}>Close Gallery</Button>
        </div>
      )}
    </Box>
  );
};

export default UserReview;
