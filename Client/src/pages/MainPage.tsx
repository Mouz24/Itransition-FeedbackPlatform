import React, { useEffect, useMemo, useState } from 'react';
import { Box, Grid, Typography, Button } from '@mui/material';
import TagCloud from '../components/TagCloud';
import { useUserContext } from '../components/UserContext';
import { Review, Tag } from '../props/Entities';
import axiosInstance from '../components/AxiosInstance';
import SearchBar from '../components/SearchBar';
import ReviewItem from '../components/ReviewItem';
import Reviews from '../components/Reviews';
import HighestMarkedReviews from '../components/Reviews';

const MainPage: React.FC = () => {
  const [selectedTags, setSelectedTags] = useState<Tag[]>([]);
  const [selectedTagsIds, setSelectedTagsIds] = useState<number[]>([]);
  const [searchResults, setSearchResults] = useState<Review[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [searched, setSearched] = useState(false);
  const { loggedInUser } = useUserContext();
  const [isMobileView, setIsMobileView] = useState<boolean>(window.innerWidth < 850);
  const [showAllReviews, setShowAllReviews] = useState<boolean>(true);

  useEffect(() => {
    setSelectedTagsIds(selectedTags.map(tag => tag.id));
  }, [selectedTags]);

  useEffect(() => {
    const handleResize = () => {
      const isMobile = window.matchMedia("(max-width: 850px)").matches;
      setIsMobileView(isMobile);
    };

    handleResize();
    window.addEventListener('resize', handleResize);

    return () => {
      window.removeEventListener('resize', handleResize);
    };
  }, []);

  const handleTagSelection = (selectedTag: Tag) => {
    const updatedTags = [...selectedTags, selectedTag];
    const updatedTagIds = updatedTags.map(tag => tag.id);
    setSelectedTags(updatedTags);
    setSelectedTagsIds(updatedTagIds);
  };
  
  const handleRemoveTag = (tagToRemove: number) => {
    const updatedTags = selectedTags.filter(tag => tag.id !== tagToRemove);
    const updatedTagIds = updatedTags.map(tag => tag.id);
    setSelectedTags(updatedTags);
    setSelectedTagsIds(updatedTagIds);
  };

  const handleSearch = async (searchTerm: string) => {
    console.log(searchTerm);
    if (searchTerm.length === 0) {
      setSearched(false);
    } else {
      const response = await axiosInstance.get<Review[]>(`search?word=${searchTerm}`);
      setSearchResults(response.data);
      setSearched(true);
    }
  };

  return (
    <div>
      <SearchBar onSearch={handleSearch} />
      <Box sx={{
        display: "grid",
        gridTemplateColumns: "repeat(3, 1fr)",
        gap: "30px",
        
        "@media (max-width: 850px)": {
          gridTemplateColumns: "1fr",
        }
      }}>
        <Grid item>
          <TagCloud onSelectTag={handleTagSelection} selectedTags={selectedTags} handleRemoveTag={handleRemoveTag}/>
        </Grid>
          {searched ? (
            searchResults.length === 0 ? (
              <p>No results found.</p>
            ) : (
              <div>
                {searchResults.map((review) => (
                  <ReviewItem key={review.id} review={review} loggedInUserId={loggedInUser?.id} />    
              ))}
              </div>
            )
          )
          : isMobileView ? (
            <>
              <Grid item xs={6}>
                <Button
                  variant={showAllReviews ? "contained" : "outlined"}
                  onClick={() => setShowAllReviews(true)}
                  fullWidth
                >
                  All Reviews
                </Button>
              </Grid>
              <Grid item xs={6}>
                <Button
                  variant={!showAllReviews ? "contained" : "outlined"}
                  onClick={() => setShowAllReviews(false)}
                  fullWidth
                >
                  Highest Marked Reviews
                </Button>
              </Grid>
              {showAllReviews && (
                <Grid item xs={12}>
                  <Reviews goal={'all'} isLoading={isLoading} setIsLoading={setIsLoading} loggedInUserId={loggedInUser?.id} tagIds={selectedTagsIds} />
                </Grid>
              )}
              {!showAllReviews && (
                <Grid item xs={12}>
                  <HighestMarkedReviews goal={'highest-marked'} isLoading={isLoading} setIsLoading={setIsLoading} loggedInUserId={loggedInUser?.id} tagIds={selectedTagsIds} />
                </Grid>
              )}
            </>
          ) : (
            <>
              <Grid item xs={12}>
                <Typography fontFamily={'monospace'} variant='h5'>
                  All Reviews
                </Typography>
                <Reviews goal={'all'} isLoading={isLoading} setIsLoading={setIsLoading} loggedInUserId={loggedInUser?.id} tagIds={selectedTagsIds} />
              </Grid>
              <Grid item xs={12}>
                <Typography fontFamily={'monospace'} variant='h5'>
                  Highest Marked Reviews
                </Typography>
                <HighestMarkedReviews goal={'highest-marked'} isLoading={isLoading} setIsLoading={setIsLoading} loggedInUserId={loggedInUser?.id} tagIds={selectedTagsIds} />
              </Grid>
            </>
          )}
      </Box>
    </div>
  );
};

export default MainPage;
