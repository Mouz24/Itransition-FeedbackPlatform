import React, { useEffect, useState } from 'react';
import { TagCloud } from 'react-tagcloud';
import { Tag } from './Entities';
import axiosInstance from './AxiosInstance';
import { Box, Chip, Typography } from '@mui/material';
import './App.css'

interface TagCloudProps {
  onSelectTag: (selectedTag: Tag) => void;
  selectedTags: Tag[];
  handleRemoveTag: (tagId: number) => void;
}

const TagCloudComponent: React.FC<TagCloudProps> = ({ onSelectTag, selectedTags, handleRemoveTag }) => {
  const [tags, setTags] = useState<Tag[]>([]);

  useEffect(() => {
    fetchTags();
  }, []);

  const fetchTags = async () => {
    const fetchedTags = await axiosInstance.get<Tag[]>('tag');
    setTags(fetchedTags.data);
  }

  const handleTagClick = async (selectedTag: Tag) => {
      onSelectTag(selectedTag);
  };

  return (
    <div>
      <TagCloud
        tags={tags}
        onClick={handleTagClick}
        minSize={20}
        maxSize={35}
        className="simple-cloud"
      />
      <Box mt={2}>
        <Typography variant="subtitle1">Selected Tags:</Typography>
        {selectedTags.map((tag) => (
          <Chip
          key={tag.id}
          label={tag.value}
          onDelete={() => handleRemoveTag(tag.id)}
          color="primary"
          variant="outlined"
          style={{ marginRight: '8px', marginBottom: '8px' }}
        />
        ))}
      </Box>
    </div>
  );
};

export default TagCloudComponent;
