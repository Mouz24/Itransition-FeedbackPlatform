import React, { useState } from 'react';
import { Link, Route, Routes, useNavigate } from 'react-router-dom';
import { Grid, Typography, Box, Avatar, Rating, Chip } from '@mui/material'; // Import Material-UI components
import AllReviews from './AllReviews';
import HighestMarkedReviews from './HighestMarkedReviews';
import TagCloud from './TagCloud';
import { useUserContext } from './UserContext';
import { Review, Tag } from './Entities';
import axiosInstance from './AxiosInstance';
// import SearchBar from './SearchBar';

const MainPage: React.FC = () => {
  const [selectedTags, setSelectedTags] = useState<Tag[]>([]);
  const [selectedTagsIds, setSelectedTagsIds] = useState<number[]>([]);
  const [searchResults, setSearchResults] = useState<Review[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [searched, setSearched] = useState(false);
  const { loggedInUser } = useUserContext();

  const handleTagSelection = (selectedTag: Tag) => {
    if (!selectedTags.includes(selectedTag)) {
      setSelectedTags([...selectedTags, selectedTag]);
      setSelectedTagsIds(getSelectedTagsIds());
    }
  };

  const getSelectedTagsIds = () => {
    return selectedTags.map((tag) => tag.id);
  }

  const handleRemoveTag = (tagToRemove : number) => {
    const updatedTags = selectedTags.filter((tag) => tag.id !== tagToRemove);
    setSelectedTags(updatedTags);
  };

  const handleSearch = async (searchTerm: string) => {
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
      {/* <SearchBar onSearch={handleSearch} /> */}
      <Grid container spacing={2}>
        <Grid item xs={12} sm={4}>
          <TagCloud onSelectTag={handleTagSelection} selectedTags={selectedTags} handleRemoveTag={handleRemoveTag}/>
        </Grid>

        <Grid item xs={12} sm={8}>
          {searched ? (
            searchResults.length === 0 ? (
              <p>No results found.</p>
            ) : (
              <div>
                {searchResults.map((review) => (
                  <div key={review.id} style={{
                    border: '2px solid #ccc',
                    borderRadius: '10px',
                    padding: '16px',
                    margin: '16px 0',
                    display: 'flex',
                    alignItems: 'center',
                  }}>
                    <Link to={`/${review.user.id}/reviews`}>
                      <Avatar src={review.user.avatar} alt={review.user.username} style={{ marginRight: '16px' }}/>
                    </Link>
                    <Link to={`/${review.user.id}/reviews`}>
                      <Typography variant="body2">{review.user.username}</Typography>
                    </Link>
                    <div style={{ flex: 1 }}>
                    <Link to={`/${review.user.id}/reviews/${review.id}`}>
                      <Typography variant="h6">{review.title}</Typography>
                    </Link>
                      <Typography variant="body2">{review.artwork.name}</Typography>
                      <Box component="fieldset" borderColor="transparent">
                      <Rating
                        name="simple-controlled"
                        value={review.artwork.rate}
                        readOnly
                      />
                      </Box>
                    </div>
                    <div>
                      <Typography variant="body2">{review.group.name}</Typography>
                    </div>
                  </div>
                ))}
              </div>
            )
          ) : (
            <div>
              <h2>All Reviews</h2>
              <AllReviews isLoading={isLoading} setIsLoading={setIsLoading} loggedInUserId={loggedInUser?.id} tagIds={selectedTagsIds} />
              <h2>Popular Reviews</h2>
              <HighestMarkedReviews isLoading={isLoading} setIsLoading={setIsLoading} loggedInUserId={loggedInUser?.id} tagIds={selectedTagsIds} />
            </div>
          )}
        </Grid>
      </Grid>
    </div>
  );
};

export default MainPage;
